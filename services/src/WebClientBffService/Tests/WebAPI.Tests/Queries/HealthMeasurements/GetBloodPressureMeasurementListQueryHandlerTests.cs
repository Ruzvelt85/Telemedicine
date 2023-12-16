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
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements.BloodPressure;
using HealthMeasurementDomain = Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure.Dto;
using Telemedicine.Services.WebClientBffService.WebAPI.Services;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Queries.HealthMeasurements
{
    public class GetBloodPressureMeasurementListQueryHandlerTests : GetHealthMeasurementListBaseQueryHandlerTests
    {
        private static readonly DateTime _now = DateTime.UtcNow;
        private static readonly DateTime _startDateTime = _now;
        private readonly PagedListResponseDto<HealthMeasurementDomain.MeasurementResponseDto<BloodPressureMeasurementDto>> _bloodPressureListResponseDto;

        public GetBloodPressureMeasurementListQueryHandlerTests()
        {
            _bloodPressureListResponseDto = GetBloodPressureListResponseDto();
        }

        [Fact]
        public async Task HandleAsync_GetBloodPressureMeasurementList_Success_Test()
        {
            //Arrange
            var query = new GetBloodPressureMeasurementListQuery
            {
                Filter = new HealthMeasurementDomain.MeasurementListFilterRequestDto(),
                Paging = new PagingRequestDto()
            };

            Mock<IBloodPressureMeasurementQueryService> bloodPressureMeasurementQueryServiceMock = new();
            bloodPressureMeasurementQueryServiceMock
                .Setup(m => m.GetBloodPressureList(It.IsAny<HealthMeasurementDomain.GetMeasurementListRequestDto>(), CancellationToken.None))
                .ReturnsAsync(_bloodPressureListResponseDto);

            var webClientBffQueryServiceMock = GetWebClientBffQueryServiceMock();

            var queryHandler = new GetBloodPressureMeasurementListQueryHandler(Mapper,
                    bloodPressureMeasurementQueryServiceMock.Object,
                    new HealthMeasurementAccessProvider(webClientBffQueryServiceMock));

            //Act
            var response = await queryHandler.HandleAsync(query);

            //Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Items);
            Assert.Equal(5, response.Items.Count);

            var expectedBloodPressureItems = _bloodPressureListResponseDto.Items.ToList();
            var actualBloodPressureItems = response.Items.OfType<BloodPressureResponseDto>().ToList();

            CheckBloodPressureResultsForEquality(expectedBloodPressureItems, actualBloodPressureItems);
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task HandleAsync_GetBloodPressureMeasurementList_ShouldInvokeWithAppointmentEndDate_Test(Range<DateTime?>? expectedRange, Range<DateTime?> requestRange)
        {
            //Arrange
            var query = new GetBloodPressureMeasurementListQuery
            {
                Filter = new HealthMeasurementDomain.MeasurementListFilterRequestDto { DateRange = requestRange },
                Paging = new PagingRequestDto()
            };

            Mock<IBloodPressureMeasurementQueryService> bloodPressureMeasurementQueryServiceMock = new();
            bloodPressureMeasurementQueryServiceMock
                .Setup(m => m.GetBloodPressureList(It.IsAny<HealthMeasurementDomain.GetMeasurementListRequestDto>(), CancellationToken.None))
                .ReturnsAsync(_bloodPressureListResponseDto);

            IWebClientBffQueryService webClientBffQueryServiceMock = GetWebClientBffQueryServiceMock();

            var queryHandler = new GetBloodPressureMeasurementListQueryHandler(Mapper,
                bloodPressureMeasurementQueryServiceMock.Object,
                new HealthMeasurementAccessProvider(webClientBffQueryServiceMock));

            //Act
            await queryHandler.HandleAsync(query);

            //Assert
            CheckQueryHandlerInvokedWithDateRangeTo(bloodPressureMeasurementQueryServiceMock, expectedRange);
        }

        private PagedListResponseDto<HealthMeasurementDomain.MeasurementResponseDto<BloodPressureMeasurementDto>> GetBloodPressureListResponseDto()
        {
            return new PagedListResponseDto<HealthMeasurementDomain.MeasurementResponseDto<BloodPressureMeasurementDto>>()
            {
                Paging = new PagingResponseDto(5),
                Items = new List<HealthMeasurementDomain.MeasurementResponseDto<BloodPressureMeasurementDto>>
                {
                    GetBloodPressureResponseDto(Guid.NewGuid(), _now, 120, 70, 60),
                    GetBloodPressureResponseDto(Guid.NewGuid(), _now.AddDays(-1), 121, 71, 61),
                    GetBloodPressureResponseDto(Guid.NewGuid(), _now.AddDays(-2), 122, 72, 62),
                    GetBloodPressureResponseDto(Guid.NewGuid(), _now.AddDays(-3), 123, 73, 63),
                    GetBloodPressureResponseDto(Guid.NewGuid(), _now.AddDays(-4), 124, 74, 64)
                }
            };
        }

        private HealthMeasurementDomain.MeasurementResponseDto<BloodPressureMeasurementDto> GetBloodPressureResponseDto(
            Guid id, DateTime clientDate, int systolic, int diastolic, int pulseRate)
        {
            return new HealthMeasurementDomain.MeasurementResponseDto<BloodPressureMeasurementDto>
            {
                Id = id,
                ClientDate = clientDate,
                Measure = new BloodPressureMeasurementDto
                {
                    Systolic = systolic,
                    Diastolic = diastolic,
                    PulseRate = pulseRate
                }
            };
        }

        private void CheckBloodPressureResultsForEquality(List<HealthMeasurementDomain.MeasurementResponseDto<BloodPressureMeasurementDto>> expectedBloodPressureItems,
            List<BloodPressureResponseDto> actualBloodPressureItems)
        {
            for (var i = 0; i < actualBloodPressureItems.Count; i++)
            {
                Assert.Equal(expectedBloodPressureItems[i].Id, actualBloodPressureItems[i].Id);
                Assert.Equal(expectedBloodPressureItems[i].ClientDate, actualBloodPressureItems[i].ClientDate);
                Assert.Equal(expectedBloodPressureItems[i].Measure.Systolic, actualBloodPressureItems[i].Systolic);
                Assert.Equal(expectedBloodPressureItems[i].Measure.Diastolic, actualBloodPressureItems[i].Diastolic);
                Assert.Equal(expectedBloodPressureItems[i].Measure.PulseRate, actualBloodPressureItems[i].PulseRate);
            }
        }

        private static void CheckQueryHandlerInvokedWithDateRangeTo(Mock<IBloodPressureMeasurementQueryService> mock, Range<DateTime?>? expectedRange)
        {
            if (expectedRange is null)
            {
                mock.Verify(s =>
                    s.GetBloodPressureList(It.IsAny<HealthMeasurementDomain.GetMeasurementListRequestDto>(), CancellationToken.None), Times.Never);
            }
            else
            {
                mock.Verify(s =>
                    s.GetBloodPressureList(It.Is<HealthMeasurementDomain.GetMeasurementListRequestDto>(q => q.Filter.DateRange == expectedRange), CancellationToken.None));
            }
        }
    }
}
