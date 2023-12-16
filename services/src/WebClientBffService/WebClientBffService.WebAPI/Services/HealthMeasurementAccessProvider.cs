using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Services
{
    public class HealthMeasurementAccessProvider : IHealthMeasurementAccessProvider
    {
        private const string FilterToIsGreaterThanLastAppntEndTimeLogMsg = "Since measurements can only be received until the end of the last ({Ongoing}, {Missed}, {Done}) appointment, " +
                                                                           "the value of Filter.DateRange.To was changed from {FilterDateRangeTo} to {LastAppointmentEndTime} value";
        private readonly IWebClientBffQueryService _appointmentQueryService;
        private readonly ILogger _logger;

        public HealthMeasurementAccessProvider(IWebClientBffQueryService appointmentQueryService)
        {
            _appointmentQueryService = appointmentQueryService ?? throw new ArgumentNullException(nameof(appointmentQueryService));
            _logger = Log.ForContext<HealthMeasurementAccessProvider>();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public async Task<Range<DateTime?>?> TrimDateFilterAccordingToLastAppointmentAsync(Guid patientId, Range<DateTime?> dateRange, CancellationToken cancellationToken = default)
        {
            AppointmentResponseDto? lastAppointment = await GetLastCompletedAppointment(patientId, cancellationToken);
            if (lastAppointment is null)
            {
                _logger.Debug("The patient {Id} doesn't have completed appointments yet", patientId);
                return null;
            }

            DateTime lastAppointmentEndTime = lastAppointment.StartDate + lastAppointment.Duration;
            if (dateRange.From > lastAppointmentEndTime)
            {
                _logger.Debug("Last appointment end date is earlier than filtered DateRange To property value");
                return null;
            }

            if (dateRange.To is null || dateRange.To > lastAppointmentEndTime)
            {
                _logger.Debug(FilterToIsGreaterThanLastAppntEndTimeLogMsg, AppointmentState.Ongoing, AppointmentState.Missed, AppointmentState.Done, dateRange.To, lastAppointmentEndTime);
                return dateRange with { To = lastAppointmentEndTime };
            }

            return dateRange;
        }

        private async Task<AppointmentResponseDto?> GetLastCompletedAppointment(Guid patientId, CancellationToken cancellationToken)
        {
            var lastAppointmentResponse = await _appointmentQueryService.GetAppointmentList(new AppointmentListRequestDto
            {
                Filter = new AppointmentListFilterRequestDto
                {
                    AppointmentStates = new[] { AppointmentState.Ongoing, AppointmentState.Missed, AppointmentState.Done },
                    AttendeeId = patientId
                },
                Paging = PagingRequestDto.FirstItem
            }, cancellationToken);

            return lastAppointmentResponse.Items.FirstOrDefault();
        }
    }
}
