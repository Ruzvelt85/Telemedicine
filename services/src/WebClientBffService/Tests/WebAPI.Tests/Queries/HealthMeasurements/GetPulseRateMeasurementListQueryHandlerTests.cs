using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService;
using Telemedicine.Services.WebClientBffService.API.HealthMeasurements.HealthMeasurementQueryService.Dto;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements.PulseRate;
using HealthMeasurementDomain = Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate.Dto;
using Telemedicine.Services.WebClientBffService.WebAPI.Services;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Queries.HealthMeasurements
{
    public class GetPulseRateMeasurementListQueryHandlerTests : GetHealthMeasurementListBaseQueryHandlerTests
    {
        private static readonly DateTime _now = DateTime.UtcNow;
        private readonly PagedListResponseDto<HealthMeasurementDomain.MeasurementResponseDto<PulseRateMeasurementDto>> _pulseRateListResponseDto;

        public GetPulseRateMeasurementListQueryHandlerTests()
        {
            _pulseRateListResponseDto = GetPulseRateListResponseDto();
        }

        [Fact]
        public async Task HandleAsync_GetPulseRateMeasurementList_Success_Test()
        {
            //Arrange
            var query = new GetPulseRateMeasurementListQuery
            {
                Filter = new HealthMeasurementDomain.MeasurementListFilterRequestDto(),
                Paging = new PagingRequestDto()
            };

            Mock<IHealthMeasurementAccessProvider> healthMeasurementAccessProviderMock = new();
            healthMeasurementAccessProviderMock
                .Setup(m => m.TrimDateFilterAccordingToLastAppointmentAsync(It.IsAny<Guid>(), It.IsAny<Range<DateTime?>>(), CancellationToken.None))
                .ReturnsAsync((Guid _, Range<DateTime?> dateRange, CancellationToken _) => dateRange);

            Mock<IPulseRateMeasurementQueryService> pulseRateMeasurementQueryServiceMock = new();
            pulseRateMeasurementQueryServiceMock
                .Setup(m => m.GetPulseRateList(It.IsAny<HealthMeasurementDomain.GetMeasurementListRequestDto>(), CancellationToken.None))
                .ReturnsAsync(_pulseRateListResponseDto);

            var queryHandler = new GetPulseRateMeasurementListQueryHandler(Mapper,
                pulseRateMeasurementQueryServiceMock.Object, healthMeasurementAccessProviderMock.Object);

            //Act
            var response = await queryHandler.HandleAsync(query);

            //Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Items);
            Assert.Equal(5, response.Items.Count);

            var expectedPulseRateItems = _pulseRateListResponseDto.Items.ToList();
            var actualPulseRateItems = response.Items.OfType<PulseRateResponseDto>().ToList();

            CheckPulseRateResultsForEquality(expectedPulseRateItems, actualPulseRateItems);
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task HandleAsync_GetPulseRateMeasurementListQueryHandlerTests_ShouldInvokeWithAppointmentEndDate_Test(Range<DateTime?>? expectedRange, Range<DateTime?> requestRange)
        {
            //Arrange
            var query = new GetPulseRateMeasurementListQuery
            {
                Filter = new HealthMeasurementDomain.MeasurementListFilterRequestDto { DateRange = requestRange },
                Paging = new PagingRequestDto()
            };

            Mock<IPulseRateMeasurementQueryService> pulseRateMeasurementQueryServiceMock = new();
            pulseRateMeasurementQueryServiceMock
                .Setup(m => m.GetPulseRateList(It.IsAny<HealthMeasurementDomain.GetMeasurementListRequestDto>(), CancellationToken.None))
                .ReturnsAsync(_pulseRateListResponseDto);

            IWebClientBffQueryService webClientBffQueryServiceMock = GetWebClientBffQueryServiceMock();

            var queryHandler = new GetPulseRateMeasurementListQueryHandler(Mapper,
                pulseRateMeasurementQueryServiceMock.Object,
                new HealthMeasurementAccessProvider(webClientBffQueryServiceMock));

            //Act
            await queryHandler.HandleAsync(query);

            //Assert
            CheckQueryHandlerInvokedWithDateRangeTo(pulseRateMeasurementQueryServiceMock, expectedRange);
        }

        private PagedListResponseDto<HealthMeasurementDomain.MeasurementResponseDto<PulseRateMeasurementDto>> GetPulseRateListResponseDto()
        {
            return new PagedListResponseDto<HealthMeasurementDomain.MeasurementResponseDto<PulseRateMeasurementDto>>()
            {
                Paging = new PagingResponseDto(5),
                Items = new List<HealthMeasurementDomain.MeasurementResponseDto<PulseRateMeasurementDto>>
                {
                    GetPulseRateResponseDto(Guid.NewGuid(), _now, 60),
                    GetPulseRateResponseDto(Guid.NewGuid(), _now.AddDays(-1), 61),
                    GetPulseRateResponseDto(Guid.NewGuid(), _now.AddDays(-2), 62),
                    GetPulseRateResponseDto(Guid.NewGuid(), _now.AddDays(-3), 63),
                    GetPulseRateResponseDto(Guid.NewGuid(), _now.AddDays(-4), 64)
                }
            };
        }

        private HealthMeasurementDomain.MeasurementResponseDto<PulseRateMeasurementDto> GetPulseRateResponseDto(
            Guid id, DateTime clientDate, int pulseRate)
        {
            return new HealthMeasurementDomain.MeasurementResponseDto<PulseRateMeasurementDto>
            {
                Id = id,
                ClientDate = clientDate,
                Measure = new PulseRateMeasurementDto
                {
                    PulseRate = pulseRate
                }
            };
        }

        private void CheckPulseRateResultsForEquality(List<HealthMeasurementDomain.MeasurementResponseDto<PulseRateMeasurementDto>> expectedPulseRateItems,
            List<PulseRateResponseDto> actualPulseRateItems)
        {
            for (var i = 0; i < actualPulseRateItems.Count; i++)
            {
                Assert.Equal(expectedPulseRateItems[i].Id, actualPulseRateItems[i].Id);
                Assert.Equal(expectedPulseRateItems[i].ClientDate, actualPulseRateItems[i].ClientDate);
                Assert.Equal(expectedPulseRateItems[i].Measure.PulseRate, actualPulseRateItems[i].PulseRate);
            }
        }

        private static void CheckQueryHandlerInvokedWithDateRangeTo(Mock<IPulseRateMeasurementQueryService> mock, Range<DateTime?>? expectedRange)
        {
            if (expectedRange is null)
            {
                mock.Verify(s =>
                    s.GetPulseRateList(It.IsAny<HealthMeasurementDomain.GetMeasurementListRequestDto>(), CancellationToken.None), Times.Never);
            }
            else
            {
                mock.Verify(s =>
                    s.GetPulseRateList(It.Is<HealthMeasurementDomain.GetMeasurementListRequestDto>(q => q.Filter.DateRange == expectedRange), CancellationToken.None));
            }
        }
    }
}
