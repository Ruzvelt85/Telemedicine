using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Dto;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Exceptions;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Specifications;
using Telemedicine.Common.Infrastructure.Patterns.Query;

namespace Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo
{
    public class GetAppointmentConnectionInfoQueryHandler : IQueryHandler<GetAppointmentConnectionInfoQuery, AppointmentConnectionInfoResponseDto>
    {
        private readonly IOptionsSnapshot<VideoConferenceConnectionSettings> _settings;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IAppointmentConnectionInfoProvider _appointmentConnectionInfoProvider;

        public GetAppointmentConnectionInfoQueryHandler(IOptionsSnapshot<VideoConferenceConnectionSettings> settings, ICurrentUserProvider currentUserProvider,
            IAppointmentConnectionInfoProvider appointmentConnectionInfoProvider)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
            _appointmentConnectionInfoProvider = appointmentConnectionInfoProvider ?? throw new ArgumentNullException(nameof(appointmentConnectionInfoProvider));
        }

        /// <summary>
        /// Handles the query to get connection info to the conference
        /// </summary>
        /// <exception cref="AppointmentConnectionInfoRequestedTooEarlyException">Thrown when it's too early to request the connection info for the planned conference</exception>
        /// <exception cref="AppointmentConnectionInfoRequestedTooLateException">Thrown when it's too late to request the connection info for the planned conference</exception>
        /// <exception cref="AppointmentConnectionInfoRequestedByWrongUserException">Thrown when the connection info is requested by the user that is not an attendee of the appointment</exception>
        public async Task<AppointmentConnectionInfoResponseDto> HandleAsync(GetAppointmentConnectionInfoQuery query, CancellationToken cancellationToken = default)
        {
            AppointmentInfoDto appointmentResponse = await _appointmentConnectionInfoProvider.GetAppointmentInfoAsync(query.Id, cancellationToken);

            if (new TooEarlyToGetConnectionInfoSpecification(_settings.Value.TimeInSecondsBeforeAppointmentStartWhenGettingConnectionInfoAllowed).IsSatisfiedBy(appointmentResponse))
            {
                throw new AppointmentConnectionInfoRequestedTooEarlyException(appointmentResponse.Id, appointmentResponse.StartDate);
            }

            if (new TooLateToGetConnectionInfoSpecification().IsSatisfiedBy(appointmentResponse))
            {
                throw new AppointmentConnectionInfoRequestedTooLateException(appointmentResponse.Id, appointmentResponse.StartDate, appointmentResponse.StartDate.Add(appointmentResponse.Duration));
            }

            if (new UserIsNotAnAttendeeOfAppointmentSpecification(_currentUserProvider.GetId()).IsSatisfiedBy(appointmentResponse))
            {
                throw new AppointmentConnectionInfoRequestedByWrongUserException(appointmentResponse.Id, _currentUserProvider.GetId());
            }

            return await _appointmentConnectionInfoProvider.GetConnectionInfoAsync(query.Id, cancellationToken);
        }
    }
}
