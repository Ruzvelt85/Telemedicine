using System;
using System.Threading.Tasks;
using Xunit;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.HealthMeasurementDomainService.DAL;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto;
using Refit;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.IntegrationTests
{
    [Trait("Category", "Integration")]
    [Collection("Integration")]
    public sealed class MoodMeasurementCommandServiceIntegrationTests
        : IDisposable, IHttpServiceTests<IMoodMeasurementCommandService>, IDbContextTests<HealthMeasurementDomainServiceDbContext>
    {
        public HttpServiceFixture<IMoodMeasurementCommandService> HttpServiceFixture { get; }
        public EfCoreContextFixture<HealthMeasurementDomainServiceDbContext> EfCoreContextFixture { get; }
        public HealthMeasurementDomainServiceDbContext DbContext { get; }
        public IMoodMeasurementCommandService Service { get; }

        public MoodMeasurementCommandServiceIntegrationTests(
            HttpServiceFixture<IMoodMeasurementCommandService> httpServiceFixture,
            EfCoreContextFixture<HealthMeasurementDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            DbContext = EfCoreContextFixture.DbContext;
            Service = HttpServiceFixture.GetRestService();
        }

        [Fact]
        public async Task CreateMoodMeasurement_SimpleCall_ShouldReturnCreatedEntity()
        {
            var request = new CreateMeasurementRequestDto<MoodMeasurementDto>
            {
                PatientId = Guid.NewGuid(),
                ClientDate = new DateTime(2022, 04, 05, 10, 20, 30),
                Measure = new MoodMeasurementDto { Measure = MoodMeasureType.Happy }
            };

            var newMoodMeasurementId = await Service.CreateAsync(request);
            var newMoodMeasurement = await DbContext.MoodMeasurements.FindAsync(newMoodMeasurementId);

            Assert.NotNull(newMoodMeasurement);
            Assert.Equal(newMoodMeasurement.Id, newMoodMeasurementId);
            Assert.Equal(request.ClientDate, newMoodMeasurement.ClientDate);
            Assert.Equal((int)request.Measure.Measure, (int)newMoodMeasurement.Measure);
            Assert.Equal(request.PatientId, newMoodMeasurement.PatientId);
        }

        [Theory]
        [InlineData("2021-12-01 05:59:59", "2021-12-01 06:00:00")]
        [InlineData("2021-12-01 06:00:00", "2021-12-02 06:00:00")]
        [InlineData("2021-12-01 06:00:01", "2021-12-02 06:00:00")]
        public async Task CreateMoodMeasurement_TwiceInDifferentDays_ByCSTTimeZone_ShouldReturnCreatedEntity(DateTime firstDateTime, DateTime secondDateTime)
        {
            // Arrange
            var firstRequest = new CreateMeasurementRequestDto<MoodMeasurementDto>
            {
                PatientId = Guid.NewGuid(),
                ClientDate = firstDateTime,
                Measure = new MoodMeasurementDto() { Measure = MoodMeasureType.Happy }
            };
            var secondRequest = new CreateMeasurementRequestDto<MoodMeasurementDto>
            {
                PatientId = firstRequest.PatientId,
                ClientDate = secondDateTime,
                Measure = new MoodMeasurementDto() { Measure = MoodMeasureType.Happy }
            };

            // Act
            var firstCreatedId = await Service.CreateAsync(firstRequest);
            var secondCreatedId = await Service.CreateAsync(secondRequest);

            // Assert
            var newFirstMeasurement = await DbContext.MoodMeasurements.FindAsync(firstCreatedId);
            var newSecondMeasurement = await DbContext.MoodMeasurements.FindAsync(secondCreatedId);
            Assert.NotNull(newFirstMeasurement);
            Assert.Equal(firstRequest.PatientId, newFirstMeasurement.PatientId);
            Assert.Equal((int)firstRequest.Measure.Measure, (int)newFirstMeasurement.Measure);
            Assert.Equal(firstRequest.ClientDate, newFirstMeasurement.ClientDate);
            Assert.NotNull(newSecondMeasurement);
            Assert.Equal(secondRequest.PatientId, newSecondMeasurement.PatientId);
            Assert.Equal((int)secondRequest.Measure.Measure, (int)newSecondMeasurement.Measure);
            Assert.Equal(secondRequest.ClientDate, newSecondMeasurement.ClientDate);
        }

        [Theory]
        [InlineData("2021-12-01 06:00:00", "2021-12-01 06:00:00")]
        [InlineData("2021-12-01 06:00:00", "2021-12-01 06:00:01")]
        [InlineData("2021-12-01 06:00:00", "2021-12-02 05:59:59")]
        public async Task CreateMoodMeasurement_TwiceInOneDay_ByCSTTimeZone_ShouldThrowMoodAlreadyCreatedTodayException(DateTime firstDateTime, DateTime secondDateTime)
        {
            var firstRequest = new CreateMeasurementRequestDto<MoodMeasurementDto>
            {
                PatientId = Guid.NewGuid(),
                ClientDate = firstDateTime,
                Measure = new MoodMeasurementDto { Measure = MoodMeasureType.Happy }
            };
            var secondRequest = new CreateMeasurementRequestDto<MoodMeasurementDto>
            {
                PatientId = firstRequest.PatientId,
                ClientDate = secondDateTime,
                Measure = new MoodMeasurementDto { Measure = MoodMeasureType.Happy }
            };

            var newMoodMeasurementId = await Service.CreateAsync(firstRequest);
            var newMoodMeasurement = await DbContext.MoodMeasurements.FindAsync(newMoodMeasurementId);

            Assert.NotNull(newMoodMeasurement);
            await DbContext.SaveChangesAsync();
            await Assert.ThrowsAsync<ApiException>(() => Service.CreateAsync(secondRequest));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            EfCoreContextFixture.CleanAllTablesExcept().Wait();
        }
    }
}
