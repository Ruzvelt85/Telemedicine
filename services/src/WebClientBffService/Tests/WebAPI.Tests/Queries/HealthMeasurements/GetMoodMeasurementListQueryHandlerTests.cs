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
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements.Mood;
using HealthMeasurementDomain = Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto;
using Telemedicine.Services.WebClientBffService.WebAPI.Services;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Queries.HealthMeasurements
{
    public class GetMoodMeasurementListQueryHandlerTests : GetHealthMeasurementListBaseQueryHandlerTests
    {
        private static readonly DateTime _now = DateTime.UtcNow;
        private readonly PagedListResponseDto<HealthMeasurementDomain.MeasurementResponseDto<MoodMeasurementDto>> _moodListResponseDto;

        public GetMoodMeasurementListQueryHandlerTests()
        {
            _moodListResponseDto = GetMoodListResponseDto();
        }

        [Fact]
        public async Task HandleAsync_GetMoodMeasurementList_Success_Test()
        {
            //Arrange
            var query = new GetMoodMeasurementListQuery
            {
                Filter = new HealthMeasurementDomain.MeasurementListFilterRequestDto(),
                Paging = new PagingRequestDto()
            };

            Mock<IMoodMeasurementQueryService> moodMeasurementQueryServiceMock = new();
            moodMeasurementQueryServiceMock
                .Setup(m => m.GetMoodList(It.IsAny<HealthMeasurementDomain.GetMeasurementListRequestDto>(), CancellationToken.None))
                .ReturnsAsync(_moodListResponseDto);

            Mock<IHealthMeasurementAccessProvider> healthMeasurementAccessProviderMock = new();
            healthMeasurementAccessProviderMock
                .Setup(m => m.TrimDateFilterAccordingToLastAppointmentAsync(It.IsAny<Guid>(), It.IsAny<Range<DateTime?>>(), CancellationToken.None))
                .ReturnsAsync((Guid _, Range<DateTime?> dateRange, CancellationToken _) => dateRange);

            var queryHandler = new GetMoodMeasurementListQueryHandler(Mapper,
                moodMeasurementQueryServiceMock.Object, healthMeasurementAccessProviderMock.Object);

            //Act
            var response = await queryHandler.HandleAsync(query);

            //Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Items);
            Assert.Equal(5, response.Items.Count);

            var expectedMoodItems = _moodListResponseDto.Items.ToList();
            var actualMoodItems = response.Items.OfType<MoodResponseDto>().ToList();

            CheckMoodResultsForEquality(expectedMoodItems, actualMoodItems);
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task HandleAsync_GetMoodMeasurementListQueryHandlerTests_ShouldInvokeWithAppointmentEndDate_Test(Range<DateTime?>? expectedRange, Range<DateTime?> requestRange)
        {
            //Arrange
            var query = new GetMoodMeasurementListQuery
            {
                Filter = new HealthMeasurementDomain.MeasurementListFilterRequestDto { DateRange = requestRange },
                Paging = new PagingRequestDto()
            };

            Mock<IMoodMeasurementQueryService> moodMeasurementQueryServiceMock = new();
            moodMeasurementQueryServiceMock
                .Setup(m => m.GetMoodList(It.IsAny<HealthMeasurementDomain.GetMeasurementListRequestDto>(), CancellationToken.None))
                .ReturnsAsync(_moodListResponseDto);

            IWebClientBffQueryService webClientBffQueryServiceMock = GetWebClientBffQueryServiceMock();

            var queryHandler = new GetMoodMeasurementListQueryHandler(Mapper,
                moodMeasurementQueryServiceMock.Object,
                new HealthMeasurementAccessProvider(webClientBffQueryServiceMock));

            //Act
            await queryHandler.HandleAsync(query);

            //Assert
            CheckQueryHandlerInvokedWithDateRangeTo(moodMeasurementQueryServiceMock, expectedRange);
        }

        private PagedListResponseDto<HealthMeasurementDomain.MeasurementResponseDto<MoodMeasurementDto>> GetMoodListResponseDto()
        {
            return new PagedListResponseDto<HealthMeasurementDomain.MeasurementResponseDto<MoodMeasurementDto>>()
            {
                Paging = new PagingResponseDto(5),
                Items = new List<HealthMeasurementDomain.MeasurementResponseDto<MoodMeasurementDto>>
                {
                    GetMoodResponseDto(Guid.NewGuid(), _now, MoodMeasureType.Happy),
                    GetMoodResponseDto(Guid.NewGuid(), _now.AddDays(-1), MoodMeasureType.Sad),
                    GetMoodResponseDto(Guid.NewGuid(), _now.AddDays(-2), MoodMeasureType.Satisfied),
                    GetMoodResponseDto(Guid.NewGuid(), _now.AddDays(-3), MoodMeasureType.Neutral),
                    GetMoodResponseDto(Guid.NewGuid(), _now.AddDays(-4), MoodMeasureType.Unhappy)
                }
            };
        }

        private HealthMeasurementDomain.MeasurementResponseDto<MoodMeasurementDto> GetMoodResponseDto(
            Guid id, DateTime clientDate, MoodMeasureType measure)
        {
            return new HealthMeasurementDomain.MeasurementResponseDto<MoodMeasurementDto>
            {
                Id = id,
                ClientDate = clientDate,
                Measure = new MoodMeasurementDto
                {
                    Measure = measure,
                }
            };
        }

        private void CheckMoodResultsForEquality(List<HealthMeasurementDomain.MeasurementResponseDto<MoodMeasurementDto>> expectedMoodItems,
            List<MoodResponseDto> actualMoodItems)
        {
            for (var i = 0; i < actualMoodItems.Count; i++)
            {
                Assert.Equal(expectedMoodItems[i].Id, actualMoodItems[i].Id);
                Assert.Equal(expectedMoodItems[i].ClientDate, actualMoodItems[i].ClientDate);
                Assert.Equal((int)expectedMoodItems[i].Measure.Measure, (int)actualMoodItems[i].Measure);
            }
        }

        private static void CheckQueryHandlerInvokedWithDateRangeTo(Mock<IMoodMeasurementQueryService> mock, Range<DateTime?>? expectedRange)
        {
            if (expectedRange is null)
            {
                mock.Verify(s =>
                    s.GetMoodList(It.IsAny<HealthMeasurementDomain.GetMeasurementListRequestDto>(), CancellationToken.None), Times.Never);
            }
            else
            {
                mock.Verify(s =>
                    s.GetMoodList(It.Is<HealthMeasurementDomain.GetMeasurementListRequestDto>(q => q.Filter.DateRange == expectedRange), CancellationToken.None));
            }
        }
    }
}
