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
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements.Saturation;
using HealthMeasurementDomain = Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation.Dto;
using Telemedicine.Services.WebClientBffService.WebAPI.Services;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Queries.HealthMeasurements
{
    public class GetSaturationMeasurementListQueryHandlerTests : GetHealthMeasurementListBaseQueryHandlerTests
    {
        private static readonly DateTime _now = DateTime.UtcNow;
        private readonly PagedListResponseDto<HealthMeasurementDomain.MeasurementResponseDto<SaturationMeasurementDto>> _saturationListResponseDto;

        public GetSaturationMeasurementListQueryHandlerTests()
        {
            _saturationListResponseDto = GetSaturationListResponseDto();
        }

        [Fact]
        public async Task HandleAsync_GetSaturationMeasurementList_Success_Test()
        {
            //Arrange
            var query = new GetSaturationMeasurementListQuery
            {
                Filter = new HealthMeasurementDomain.MeasurementListFilterRequestDto(),
                Paging = new PagingRequestDto()
            };

            Mock<ISaturationMeasurementQueryService> saturationMeasurementQueryServiceMock = new();
            saturationMeasurementQueryServiceMock
                .Setup(m => m.GetSaturationList(It.IsAny<HealthMeasurementDomain.GetMeasurementListRequestDto>(), CancellationToken.None))
                .ReturnsAsync(_saturationListResponseDto);

            Mock<IHealthMeasurementAccessProvider> healthMeasurementAccessProviderMock = new();
            healthMeasurementAccessProviderMock
                .Setup(m => m.TrimDateFilterAccordingToLastAppointmentAsync(It.IsAny<Guid>(), It.IsAny<Range<DateTime?>>(), CancellationToken.None))
                .ReturnsAsync((Guid _, Range<DateTime?> dateRange, CancellationToken _) => dateRange);

            var queryHandler = new GetSaturationMeasurementListQueryHandler(Mapper,
                saturationMeasurementQueryServiceMock.Object, healthMeasurementAccessProviderMock.Object);

            //Act
            var response = await queryHandler.HandleAsync(query);

            //Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Items);
            Assert.Equal(5, response.Items.Count);

            var expectedSaturationItems = _saturationListResponseDto.Items.ToList();
            var actualSaturationItems = response.Items.OfType<SaturationResponseDto>().ToList();

            CheckSaturationResultsForEquality(expectedSaturationItems, actualSaturationItems);
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task HandleAsync_GetSaturationMeasurementListQueryHandlerTests_ShouldInvokeWithAppointmentEndDate_Test(Range<DateTime?>? expectedRange, Range<DateTime?> requestRange)
        {
            //Arrange
            var query = new GetSaturationMeasurementListQuery
            {
                Filter = new HealthMeasurementDomain.MeasurementListFilterRequestDto { DateRange = requestRange },
                Paging = new PagingRequestDto()
            };

            Mock<ISaturationMeasurementQueryService> saturationMeasurementQueryServiceMock = new();
            saturationMeasurementQueryServiceMock
                .Setup(m => m.GetSaturationList(It.IsAny<HealthMeasurementDomain.GetMeasurementListRequestDto>(), CancellationToken.None))
                .ReturnsAsync(_saturationListResponseDto);

            IWebClientBffQueryService webClientBffQueryServiceMock = GetWebClientBffQueryServiceMock();

            var queryHandler = new GetSaturationMeasurementListQueryHandler(Mapper,
                saturationMeasurementQueryServiceMock.Object,
                new HealthMeasurementAccessProvider(webClientBffQueryServiceMock));

            //Act
            await queryHandler.HandleAsync(query);

            //Assert
            CheckQueryHandlerInvokedWithDateRangeTo(saturationMeasurementQueryServiceMock, expectedRange);
        }

        private PagedListResponseDto<HealthMeasurementDomain.MeasurementResponseDto<SaturationMeasurementDto>> GetSaturationListResponseDto()
        {
            return new PagedListResponseDto<HealthMeasurementDomain.MeasurementResponseDto<SaturationMeasurementDto>>()
            {
                Paging = new PagingResponseDto(5),
                Items = new List<HealthMeasurementDomain.MeasurementResponseDto<SaturationMeasurementDto>>
                {
                    GetSaturationResponseDto(Guid.NewGuid(), _now, 70, 60),
                    GetSaturationResponseDto(Guid.NewGuid(), _now.AddDays(-1), 71, 61),
                    GetSaturationResponseDto(Guid.NewGuid(), _now.AddDays(-2), 72, 62),
                    GetSaturationResponseDto(Guid.NewGuid(), _now.AddDays(-3), 73, 63),
                    GetSaturationResponseDto(Guid.NewGuid(), _now.AddDays(-4), 74, 64)
                }
            };
        }

        private HealthMeasurementDomain.MeasurementResponseDto<SaturationMeasurementDto> GetSaturationResponseDto(
            Guid id, DateTime clientDate, int spO2, int pulseRate)
        {
            return new HealthMeasurementDomain.MeasurementResponseDto<SaturationMeasurementDto>
            {
                Id = id,
                ClientDate = clientDate,
                Measure = new SaturationMeasurementDto
                {
                    SpO2 = spO2,
                    PulseRate = pulseRate
                }
            };
        }

        private void CheckSaturationResultsForEquality(List<HealthMeasurementDomain.MeasurementResponseDto<SaturationMeasurementDto>> expectedSaturationItems,
            List<SaturationResponseDto> actualSaturationItems)
        {
            for (var i = 0; i < actualSaturationItems.Count; i++)
            {
                Assert.Equal(expectedSaturationItems[i].Id, actualSaturationItems[i].Id);
                Assert.Equal(expectedSaturationItems[i].ClientDate, actualSaturationItems[i].ClientDate);
                Assert.Equal(expectedSaturationItems[i].Measure.SpO2, actualSaturationItems[i].SpO2);
                Assert.Equal(expectedSaturationItems[i].Measure.PulseRate, actualSaturationItems[i].PulseRate);
            }
        }

        private static void CheckQueryHandlerInvokedWithDateRangeTo(Mock<ISaturationMeasurementQueryService> mock, Range<DateTime?>? expectedRange)
        {
            if (expectedRange is null)
            {
                mock.Verify(s =>
                    s.GetSaturationList(It.IsAny<HealthMeasurementDomain.GetMeasurementListRequestDto>(), CancellationToken.None), Times.Never);
            }
            else
            {
                mock.Verify(s =>
                    s.GetSaturationList(It.Is<HealthMeasurementDomain.GetMeasurementListRequestDto>(q => q.Filter.DateRange == expectedRange), CancellationToken.None));
            }
        }
    }
}
