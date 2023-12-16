using System;
using System.Collections.Generic;
using System.Threading;
using AutoMapper;
using Moq;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Queries.HealthMeasurements
{
    public abstract class GetHealthMeasurementListBaseQueryHandlerTests
    {
        private static readonly DateTime _now = DateTime.UtcNow;
        private static readonly DateTime _startDateTime = _now;
        protected readonly IMapper Mapper;

        protected GetHealthMeasurementListBaseQueryHandlerTests()
        {
            Mapper = new MapperConfiguration(cfg =>
                cfg.AddMaps(typeof(WebClientBffService.WebAPI.Startup).Assembly)).CreateMapper();
        }

        public static IEnumerable<object?[]> GetTestData()
        {
            // range before appointment must be unchanged
            yield return new object?[] { new Range<DateTime?>(DateTime.MinValue, _startDateTime.AddHours(-1)),
                new Range<DateTime?>(DateTime.MinValue, _startDateTime.AddHours(-1)) };

            // range with To equal to start of appointment must be unchanged
            yield return new object?[] { new Range<DateTime?>(DateTime.MinValue, _startDateTime),
                new Range<DateTime?>(DateTime.MinValue, _startDateTime) };

            // range with From equal to start of appointment and To before end of appointment must be unchanged
            yield return new object?[] { new Range<DateTime?>(_startDateTime, _startDateTime.AddMinutes(30)),
                new Range<DateTime?>(_startDateTime, _startDateTime.AddMinutes(30)) };

            // range with From equal to start of appointment and To after end of appointment must be changed to end of appointment
            yield return new object?[] { new Range<DateTime?>(_startDateTime, _startDateTime.AddHours(1)),
                new Range<DateTime?>(_startDateTime, DateTime.MaxValue) };

            // range inside while appointment ongoing must be unchanged
            yield return new object?[] { new Range<DateTime?>(_startDateTime.AddMinutes(10), _startDateTime.AddMinutes(20)),
                new Range<DateTime?>(_startDateTime.AddMinutes(10), _startDateTime.AddMinutes(20)) };

            // range with From equal to end of appointment must be change to end datetime only
            yield return new object?[] { new Range<DateTime?>(_startDateTime.AddHours(1), _startDateTime.AddHours(1)),
                new Range<DateTime?>(_startDateTime.AddHours(1), _startDateTime.AddHours(2)) };

            // range after appointment must be null
            yield return new object?[] { null, new Range<DateTime?>(_startDateTime.AddDays(1), _startDateTime.AddDays(2)) };
        }

        protected IWebClientBffQueryService GetWebClientBffQueryServiceMock()
        {
            var webClientBffQueryServiceMock = new Mock<IWebClientBffQueryService>();
            webClientBffQueryServiceMock.Setup(_ => _.GetAppointmentList(It.IsAny<AppointmentListRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AppointmentListResponseDto
                {
                    Items = new[]
                    {
                        new AppointmentResponseDto
                        {
                            StartDate = _startDateTime,
                            Duration = TimeSpan.FromHours(1)
                        }
                    }
                });

            return webClientBffQueryServiceMock.Object;
        }
    }
}
