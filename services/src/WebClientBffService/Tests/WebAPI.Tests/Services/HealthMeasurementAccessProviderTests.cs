using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;
using Telemedicine.Services.WebClientBffService.WebAPI.Services;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Services
{
    public class HealthMeasurementAccessProviderTests
    {
        private readonly IHealthMeasurementAccessProvider _healthMeasurementAccessProvider;
        private static readonly Guid _defaultPatientId = Guid.NewGuid();
        private static readonly DateTime _startDateTime = DateTime.UtcNow;

        public HealthMeasurementAccessProviderTests()
        {
            var webClientBffQueryServiceMock = GetWebClientBffQueryServiceMock();
            _healthMeasurementAccessProvider = new HealthMeasurementAccessProvider(webClientBffQueryServiceMock);
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task AllCases_MustBeSuccess(Guid patientId, Range<DateTime?>? expectedRange, Range<DateTime?> queryRange)
        {
            //Act
            var trimmedRange = await _healthMeasurementAccessProvider.TrimDateFilterAccordingToLastAppointmentAsync(patientId, queryRange);

            //Assert
            Assert.Equal(expectedRange, trimmedRange);
        }

        public static IEnumerable<object?[]> GetTestData()
        {
            // without appointment must be null
            yield return new object?[] { Guid.NewGuid(), null, new Range<DateTime?>(DateTime.MinValue, DateTime.MaxValue) };

            // range before appointment must be unchanged
            yield return new object?[] { _defaultPatientId, new Range<DateTime?>(DateTime.MinValue, _startDateTime.AddHours(-1)),
                new Range<DateTime?>(DateTime.MinValue, _startDateTime.AddHours(-1)) };

            // range with To equal to start of appointment must be unchanged
            yield return new object?[] { _defaultPatientId, new Range<DateTime?>(DateTime.MinValue, _startDateTime),
                new Range<DateTime?>(DateTime.MinValue, _startDateTime) };

            // range with From equal to start of appointment and To before end of appointment must be unchanged
            yield return new object?[] { _defaultPatientId, new Range<DateTime?>(_startDateTime, _startDateTime.AddMinutes(30)),
                new Range<DateTime?>(_startDateTime, _startDateTime.AddMinutes(30)) };

            // range with From equal to start of appointment and To after end of appointment must be changed to end of appointment
            yield return new object?[] { _defaultPatientId, new Range<DateTime?>(_startDateTime, _startDateTime.AddHours(1)),
                new Range<DateTime?>(_startDateTime, DateTime.MaxValue) };

            // range inside while appointment ongoing must be unchanged
            yield return new object?[] { _defaultPatientId, new Range<DateTime?>(_startDateTime.AddMinutes(10), _startDateTime.AddMinutes(20)),
                new Range<DateTime?>(_startDateTime.AddMinutes(10), _startDateTime.AddMinutes(20)) };

            // range with From equal to end of appointment must be change to end datetime only
            yield return new object?[] { _defaultPatientId, new Range<DateTime?>(_startDateTime.AddHours(1), _startDateTime.AddHours(1)),
                new Range<DateTime?>(_startDateTime.AddHours(1), _startDateTime.AddHours(2)) };

            // range after appointment must be null
            yield return new object?[] { _defaultPatientId, null,
                new Range<DateTime?>(_startDateTime.AddDays(1), _startDateTime.AddDays(2)) };
        }

        private IWebClientBffQueryService GetWebClientBffQueryServiceMock()
        {
            var webClientBffQueryServiceMock = new Mock<IWebClientBffQueryService>();
            webClientBffQueryServiceMock.Setup(_ => _.GetAppointmentList(It.IsAny<AppointmentListRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AppointmentListRequestDto requestDto, CancellationToken _) => GetAppointmentListResponseDto(requestDto.Filter.AttendeeId));

            return webClientBffQueryServiceMock.Object;
        }

        private AppointmentListResponseDto GetAppointmentListResponseDto(Guid patientId)
        {
            if (patientId != _defaultPatientId)
            { return new AppointmentListResponseDto(); }

            return new AppointmentListResponseDto
            {
                Items = new[]
                {
                    new AppointmentResponseDto
                    {
                        StartDate = _startDateTime,
                        Duration = TimeSpan.FromHours(1)
                    }
                }
            };
        }
    }
}
