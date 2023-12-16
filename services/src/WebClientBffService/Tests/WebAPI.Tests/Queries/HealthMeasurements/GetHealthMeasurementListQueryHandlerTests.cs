using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.WebClientBffService.API.HealthMeasurements.HealthMeasurementQueryService.Dto;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements;
using Telemedicine.Services.WebClientBffService.API.Common;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements.BloodPressure;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements.Saturation;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements.Mood;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements.PulseRate;
using Telemedicine.Common.Infrastructure.Patterns.Query;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Queries.HealthMeasurements
{
    public class GetHealthMeasurementListQueryHandlerTests
    {
        private static readonly DateTime _now = DateTime.UtcNow;
        private static readonly DateTime _lastAppointmentDefaultStartDate = DateTime.UtcNow;
        private readonly MeasurementListResponse _bloodPressureListResponseDto;
        private readonly MeasurementListResponse _saturationListResponseDto;
        private readonly MeasurementListResponse _moodListResponseDto;
        private readonly MeasurementListResponse _pulseRateListResponseDto;
        private readonly GetHealthMeasurementListQueryHandler _queryHandler;

        public GetHealthMeasurementListQueryHandlerTests()
        {
            _bloodPressureListResponseDto = GetBloodPressureListResponseDto();
            _saturationListResponseDto = GetSaturationListResponseDto();
            _moodListResponseDto = GetMoodListResponseDto();
            _pulseRateListResponseDto = GetPulseRateListResponseDto();

            IMapper mapper = new MapperConfiguration(cfg =>
                cfg.AddMaps(typeof(WebClientBffService.WebAPI.Startup).Assembly)).CreateMapper();

            var bloodPressureMeasurementQueryHandlerMock = new Mock<IQueryHandler<GetBloodPressureMeasurementListQuery, MeasurementListResponse>>();
            bloodPressureMeasurementQueryHandlerMock
                .Setup(m => m.HandleAsync(It.IsAny<GetBloodPressureMeasurementListQuery>(), CancellationToken.None))
                .ReturnsAsync(_bloodPressureListResponseDto);

            var saturationMeasurementQueryHandlerMock = new Mock<IQueryHandler<GetSaturationMeasurementListQuery, MeasurementListResponse>>();
            saturationMeasurementQueryHandlerMock
                .Setup(m => m.HandleAsync(It.IsAny<GetSaturationMeasurementListQuery>(), CancellationToken.None))
                .ReturnsAsync(_saturationListResponseDto);

            var moodMeasurementQueryHandlerMock = new Mock<IQueryHandler<GetMoodMeasurementListQuery, MeasurementListResponse>>();
            moodMeasurementQueryHandlerMock
                .Setup(m => m.HandleAsync(It.IsAny<GetMoodMeasurementListQuery>(), CancellationToken.None))
                .ReturnsAsync(_moodListResponseDto);

            var pulseRateMeasurementQueryHandlerMock = new Mock<IQueryHandler<GetPulseRateMeasurementListQuery, MeasurementListResponse>>();
            pulseRateMeasurementQueryHandlerMock
                .Setup(m => m.HandleAsync(It.IsAny<GetPulseRateMeasurementListQuery>(), CancellationToken.None))
                .ReturnsAsync(_pulseRateListResponseDto);

            _queryHandler = new GetHealthMeasurementListQueryHandler(mapper,
                bloodPressureMeasurementQueryHandlerMock.Object,
                saturationMeasurementQueryHandlerMock.Object,
                pulseRateMeasurementQueryHandlerMock.Object,
                moodMeasurementQueryHandlerMock.Object);
        }

        [Fact]
        public async Task HandleAsync_GetHealthMeasurementList_Default_Test()
        {
            //Arrange
            var query = new GetHealthMeasurementListQuery(
                new MeasurementListFilterRequestDto(),
                new PagingRequestDto(12)
            );

            //Act
            var response = await _queryHandler.HandleAsync(query);

            //Assert
            Assert.NotNull(response);
            Assert.NotNull(response.BloodPressureItems);
            Assert.Equal(3, response.BloodPressureItems.Count);
            Assert.NotNull(response.SaturationItems);
            Assert.Equal(3, response.SaturationItems.Count);
            Assert.NotNull(response.MoodItems);
            Assert.Equal(3, response.MoodItems.Count);
            Assert.NotNull(response.PulseRateItems);
            Assert.Equal(3, response.PulseRateItems.Count);

            var expectedBloodPressureItems = _bloodPressureListResponseDto.Items.OfType<BloodPressureResponseDto>().ToList();
            var actualBloodPressureItems = response.BloodPressureItems.ToList();
            CheckBloodPressureResultsForEquality(expectedBloodPressureItems, actualBloodPressureItems);

            var expectedSaturationItems = _saturationListResponseDto.Items.OfType<SaturationResponseDto>().ToList();
            var actualSaturationItems = response.SaturationItems.ToList();
            CheckSaturationResultsForEquality(expectedSaturationItems, actualSaturationItems);

            var expectedMoodItems = _moodListResponseDto.Items.OfType<MoodResponseDto>().ToList();
            var actualMoodItems = response.MoodItems.ToList();
            CheckMoodResultsForEquality(expectedMoodItems, actualMoodItems);

            var expectedPulseRateItems = _pulseRateListResponseDto.Items.OfType<PulseRateResponseDto>().ToList();
            var actualPulseRateItems = response.PulseRateItems.ToList();
            CheckPulseRateResultsForEquality(expectedPulseRateItems, actualPulseRateItems);
        }

        [Fact]
        public async Task HandleAsync_GetHealthMeasurementList_BloodPressure_Test()
        {
            //Arrange
            var query = new GetHealthMeasurementListQuery(
                new MeasurementListFilterRequestDto
                {
                    MeasurementType = MeasurementType.BloodPressure
                },
                new PagingRequestDto()
            );

            //Act
            var response = await _queryHandler.HandleAsync(query);

            //Assert
            Assert.NotNull(response);
            Assert.NotNull(response.BloodPressureItems);
            Assert.Equal(5, response.BloodPressureItems.Count);
            Assert.NotNull(response.SaturationItems);
            Assert.Empty(response.SaturationItems);
            Assert.NotNull(response.MoodItems);
            Assert.Empty(response.MoodItems);
            Assert.NotNull(response.PulseRateItems);
            Assert.Empty(response.PulseRateItems);

            var expectedBloodPressureItems = _bloodPressureListResponseDto.Items.OfType<BloodPressureResponseDto>().ToList();
            var actualBloodPressureItems = response.BloodPressureItems.ToList();

            CheckBloodPressureResultsForEquality(expectedBloodPressureItems, actualBloodPressureItems);
        }

        [Fact]
        public async Task HandleAsync_GetHealthMeasurementList_Saturation_Test()
        {
            //Arrange
            var query = new GetHealthMeasurementListQuery(
                new MeasurementListFilterRequestDto
                {
                    MeasurementType = MeasurementType.Saturation
                },
                new PagingRequestDto()
            );

            //Act
            var response = await _queryHandler.HandleAsync(query);

            //Assert
            Assert.NotNull(response);
            Assert.NotNull(response.BloodPressureItems);
            Assert.Empty(response.BloodPressureItems);
            Assert.NotNull(response.SaturationItems);
            Assert.Equal(5, response.SaturationItems.Count);
            Assert.NotNull(response.MoodItems);
            Assert.Empty(response.MoodItems);
            Assert.NotNull(response.PulseRateItems);
            Assert.Empty(response.PulseRateItems);

            var expectedSaturationItems = _saturationListResponseDto.Items.OfType<SaturationResponseDto>().ToList();
            var actualSaturationItems = response.SaturationItems.ToList();

            CheckSaturationResultsForEquality(expectedSaturationItems, actualSaturationItems);
        }

        [Fact]
        public async Task HandleAsync_GetHealthMeasurementList_Mood_Test()
        {
            //Arrange
            var query = new GetHealthMeasurementListQuery(
                new MeasurementListFilterRequestDto
                {
                    MeasurementType = MeasurementType.Mood
                },
                new PagingRequestDto()
            );

            //Act
            var response = await _queryHandler.HandleAsync(query);

            //Assert
            Assert.NotNull(response);
            Assert.NotNull(response.BloodPressureItems);
            Assert.Empty(response.BloodPressureItems);
            Assert.NotNull(response.SaturationItems);
            Assert.Empty(response.SaturationItems);
            Assert.NotNull(response.MoodItems);
            Assert.Equal(5, response.MoodItems.Count);
            Assert.NotNull(response.PulseRateItems);
            Assert.Empty(response.PulseRateItems);

            var expectedMoodItems = _moodListResponseDto.Items.OfType<MoodResponseDto>().ToList();
            var actualMoodItems = response.MoodItems.ToList();

            CheckMoodResultsForEquality(expectedMoodItems, actualMoodItems);
        }

        [Fact]
        public async Task HandleAsync_GetHealthMeasurementList_PulseRate_Test()
        {
            //Arrange
            var query = new GetHealthMeasurementListQuery(
                new MeasurementListFilterRequestDto
                {
                    MeasurementType = MeasurementType.PulseRate
                },
                new PagingRequestDto()
            );

            //Act
            var response = await _queryHandler.HandleAsync(query);

            //Assert
            Assert.NotNull(response);
            Assert.NotNull(response.BloodPressureItems);
            Assert.Empty(response.BloodPressureItems);
            Assert.NotNull(response.SaturationItems);
            Assert.Empty(response.SaturationItems);
            Assert.NotNull(response.MoodItems);
            Assert.Empty(response.MoodItems);
            Assert.NotNull(response.PulseRateItems);
            Assert.Equal(5, response.PulseRateItems.Count);

            var expectedPulseRateItems = _pulseRateListResponseDto.Items.OfType<PulseRateResponseDto>().ToList();
            var actualPulseRateItems = response.PulseRateItems.ToList();

            CheckPulseRateResultsForEquality(expectedPulseRateItems, actualPulseRateItems);
        }

        private MeasurementListResponse GetBloodPressureListResponseDto()
        {
            return new MeasurementListResponse()
            {
                Paging = new PagingResponseDto(5),
                Items = new List<BloodPressureResponseDto>
                {
                    GetBloodPressureResponseDto(Guid.NewGuid(), _now, 120, 70, 60),
                    GetBloodPressureResponseDto(Guid.NewGuid(), _now.AddDays(-1), 121, 71, 61),
                    GetBloodPressureResponseDto(Guid.NewGuid(), _now.AddDays(-2), 122, 72, 62),
                    GetBloodPressureResponseDto(Guid.NewGuid(), _now.AddDays(-3), 123, 73, 63),
                    GetBloodPressureResponseDto(Guid.NewGuid(), _now.AddDays(-4), 124, 74, 64)
                }
            };
        }

        private MeasurementListResponse GetSaturationListResponseDto()
        {
            return new MeasurementListResponse()
            {
                Paging = new PagingResponseDto(5),
                Items = new List<SaturationResponseDto>
                {
                    GetSaturationMeasurementResponseDto(Guid.NewGuid(), _now, 80, 60),
                    GetSaturationMeasurementResponseDto(Guid.NewGuid(), _now.AddDays(-1), 90, 61),
                    GetSaturationMeasurementResponseDto(Guid.NewGuid(), _now.AddDays(-2), 70, 62),
                    GetSaturationMeasurementResponseDto(Guid.NewGuid(), _now.AddDays(-3), 65, 63),
                    GetSaturationMeasurementResponseDto(Guid.NewGuid(), _now.AddDays(-4), 85, 64)
                }
            };
        }

        private MeasurementListResponse GetMoodListResponseDto()
        {
            return new MeasurementListResponse()
            {
                Paging = new PagingResponseDto(5),
                Items = new List<MoodResponseDto>
                {
                    GetMoodMeasurementResponseDto(Guid.NewGuid(), _now, MoodMeasureType.Happy),
                    GetMoodMeasurementResponseDto(Guid.NewGuid(), _now.AddDays(-1), MoodMeasureType.Sad),
                    GetMoodMeasurementResponseDto(Guid.NewGuid(), _now.AddDays(-2), MoodMeasureType.Satisfied),
                    GetMoodMeasurementResponseDto(Guid.NewGuid(), _now.AddDays(-3), MoodMeasureType.Unhappy),
                    GetMoodMeasurementResponseDto(Guid.NewGuid(), _now.AddDays(-4), MoodMeasureType.Neutral)
                }
            };
        }

        private MeasurementListResponse GetPulseRateListResponseDto()
        {
            return new MeasurementListResponse()
            {
                Paging = new PagingResponseDto(5),
                Items = new List<PulseRateResponseDto>
                {
                    GetPulseRateMeasurementResponseDto(Guid.NewGuid(), _now, 60),
                    GetPulseRateMeasurementResponseDto(Guid.NewGuid(), _now.AddDays(-1), 61),
                    GetPulseRateMeasurementResponseDto(Guid.NewGuid(), _now.AddDays(-2), 62),
                    GetPulseRateMeasurementResponseDto(Guid.NewGuid(), _now.AddDays(-3), 63),
                    GetPulseRateMeasurementResponseDto(Guid.NewGuid(), _now.AddDays(-4), 64)
                }
            };
        }

        private BloodPressureResponseDto GetBloodPressureResponseDto(Guid id, DateTime clientDate, int systolic,
            int diastolic, int pulseRate)
        {
            return new BloodPressureResponseDto
            {
                Id = id,
                ClientDate = clientDate,
                Systolic = systolic,
                Diastolic = diastolic,
                PulseRate = pulseRate
            };
        }

        private SaturationResponseDto GetSaturationMeasurementResponseDto(Guid id, DateTime clientDate, int spO2,
            int pulseRate)
        {
            return new SaturationResponseDto
            {
                Id = id,
                ClientDate = clientDate,
                SpO2 = spO2,
                PulseRate = pulseRate
            };
        }

        private MoodResponseDto GetMoodMeasurementResponseDto(Guid id, DateTime clientDate, MoodMeasureType measure)
        {
            return new MoodResponseDto
            {
                Id = id,
                ClientDate = clientDate,
                Measure = measure
            };
        }

        private PulseRateResponseDto GetPulseRateMeasurementResponseDto(Guid id, DateTime clientDate, int pulseRate)
        {
            return new PulseRateResponseDto
            {
                Id = id,
                ClientDate = clientDate,
                PulseRate = pulseRate
            };
        }

        private void CheckBloodPressureResultsForEquality(List<BloodPressureResponseDto> expectedBloodPressureItems,
            List<BloodPressureResponseDto> actualBloodPressureItems)
        {
            for (var i = 0; i < actualBloodPressureItems.Count; i++)
            {
                Assert.Equal(expectedBloodPressureItems[i].Id, actualBloodPressureItems[i].Id);
                Assert.Equal(expectedBloodPressureItems[i].ClientDate, actualBloodPressureItems[i].ClientDate);
                Assert.Equal(expectedBloodPressureItems[i].Systolic, actualBloodPressureItems[i].Systolic);
                Assert.Equal(expectedBloodPressureItems[i].Diastolic, actualBloodPressureItems[i].Diastolic);
                Assert.Equal(expectedBloodPressureItems[i].PulseRate, actualBloodPressureItems[i].PulseRate);
            }
        }

        private void CheckSaturationResultsForEquality(List<SaturationResponseDto> expectedSaturationItems,
            List<SaturationResponseDto> actualSaturationItems)
        {
            for (var i = 0; i < actualSaturationItems.Count; i++)
            {
                Assert.Equal(expectedSaturationItems[i].Id, actualSaturationItems[i].Id);
                Assert.Equal(expectedSaturationItems[i].ClientDate, actualSaturationItems[i].ClientDate);
                Assert.Equal(expectedSaturationItems[i].SpO2, actualSaturationItems[i].SpO2);
                Assert.Equal(expectedSaturationItems[i].PulseRate, actualSaturationItems[i].PulseRate);
            }
        }

        private void CheckMoodResultsForEquality(List<MoodResponseDto> expectedSaturationItems,
            List<MoodResponseDto> actualSaturationItems)
        {
            for (var i = 0; i < actualSaturationItems.Count; i++)
            {
                Assert.Equal(expectedSaturationItems[i].Id, actualSaturationItems[i].Id);
                Assert.Equal(expectedSaturationItems[i].ClientDate, actualSaturationItems[i].ClientDate);
                Assert.Equal(expectedSaturationItems[i].Measure, actualSaturationItems[i].Measure);
            }
        }

        private void CheckPulseRateResultsForEquality(List<PulseRateResponseDto> expectedSaturationItems,
            List<PulseRateResponseDto> actualSaturationItems)
        {
            for (var i = 0; i < actualSaturationItems.Count; i++)
            {
                Assert.Equal(expectedSaturationItems[i].Id, actualSaturationItems[i].Id);
                Assert.Equal(expectedSaturationItems[i].ClientDate, actualSaturationItems[i].ClientDate);
                Assert.Equal(expectedSaturationItems[i].PulseRate, actualSaturationItems[i].PulseRate);
            }
        }
    }
}
