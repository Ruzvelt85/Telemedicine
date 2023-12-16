using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Refit;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.HealthMeasurementDomainService.DAL;
using Telemedicine.Services.MobileClientBffService.API;
using Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService;
using Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto;

namespace Telemedicine.Services.MobileClientBffService.Tests.WebAPI.IntegrationTests
{
    [Trait("Category", "Integration")]
    [Collection("Integration")]
    public class HealthMeasurementCommandServiceTests : IHttpServiceTests<IHealthMeasurementCommandService>, IDbContextTests<HealthMeasurementDomainServiceDbContext>
    {
        private readonly IHealthMeasurementCommandService _service;

        /// <inheritdoc />
        public HttpServiceFixture<IHealthMeasurementCommandService> HttpServiceFixture { get; }

        public EfCoreContextFixture<HealthMeasurementDomainServiceDbContext> EfCoreContextFixture { get; }

        public HealthMeasurementDomainServiceDbContext DbContext { get; }

        public HealthMeasurementCommandServiceTests(HttpServiceFixture<IHealthMeasurementCommandService> httpServiceFixture,
            EfCoreContextFixture<HealthMeasurementDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;

            DbContext = EfCoreContextFixture.DbContext;
            _service = HttpServiceFixture.GetRestService();
        }

        [Fact(Skip = "Test need currentUserId")]
        public async Task CreateMoodMeasurement_EmptyRequest_ShouldBeValidationError()
        {
            // Arrange
            var request = new CreateMoodMeasurementRequestDto();

            // Act and Assert
            await Assert.ThrowsAsync<ValidationApiException>(() => _service.CreateMoodMeasurementAsync(request));
        }

        [Fact(Skip = "Test need currentUserId")]
        public async Task CreateMoodMeasurement_SimpleCall_ShouldReturnCreatedEntityId()
        {
            // Arrange
            var request = new CreateMoodMeasurementRequestDto
            {
                ClientDate = DateTime.Now,
                Measure = MoodMeasureType.Happy
            };

            // Act
            var newMoodMeasurementId = await _service.CreateMoodMeasurementAsync(request);

            // Assert
            Assert.NotEqual(Guid.Empty, newMoodMeasurementId);
        }

        [Fact(Skip = "Test need currentUserId")]
        public async Task CreateSaturationMeasurement_EmptyRequest_ShouldBeValidationError()
        {
            var request = new CreateSaturationMeasurementRequestDto();
            await Assert.ThrowsAsync<ValidationApiException>(() => _service.CreateSaturationMeasurementAsync(request));
        }

        [Fact(Skip = "Test need currentUserId")]
        public async Task CreateSaturationMeasurement_EmptyRawSaturation_ShouldReturnCreatedEntityId()
        {
            // Arrange
            var pulseRate = 3;
            var now = DateTime.UtcNow;

            var request = new CreateSaturationMeasurementRequestDto
            {
                Pi = 1,
                SpO2 = 2,
                PulseRate = pulseRate,
                RawMeasurements = new List<RawSaturationItemRequestDto>(),
                ClientDate = now
            };

            // Act
            var newSaturationMeasurementId = await _service.CreateSaturationMeasurementAsync(request);

            // Assert
            Assert.NotEqual(Guid.Empty, newSaturationMeasurementId);

            var createdPulseRate = DbContext.PulseRateMeasurements.FirstOrDefault(m => m.PulseRate == pulseRate && m.ClientDate == now);
            Assert.NotNull(createdPulseRate);
        }

        [Fact(Skip = "Test need currentUserId")]
        public async Task CreateSaturationMeasurement_WithRawMeasurements_ValidationErrorByRawCount()
        {
            // Arrange
            // 101 is great then SaturationMeasurementSettings.MaxRawItemsValidationCountLimit
            var request = GetCreateSaturationMeasurementRequestDto(101);

            // Act and Assert
            await Assert.ThrowsAsync<ValidationApiException>(async () => await _service.CreateSaturationMeasurementAsync(request));
        }

        [Fact(Skip = "Test need currentUserId")]
        public async Task CreateSaturationMeasurement_WithRawMeasurements_ShouldBeCorrect()
        {
            // Arrange
            // 100 is equal SaturationMeasurementSettings.MaxRawItemsValidationCountLimit
            var request = GetCreateSaturationMeasurementRequestDto(100);

            // Act
            var createdId = await _service.CreateSaturationMeasurementAsync(request);

            // Assert
            Assert.NotEqual(Guid.Empty, createdId);
        }

        [Fact(Skip = "Test need currentUserId")]
        public async Task CreateSaturationMeasurement_SimpleCall_ShouldReturnCreatedEntityId()
        {
            // Arrange
            var request = GetCreateSaturationMeasurementRequestDto(1);

            // Act
            var newSaturationMeasurementId = await _service.CreateSaturationMeasurementAsync(request);

            // Assert
            Assert.NotEqual(Guid.Empty, newSaturationMeasurementId);

            var createdPulseRate = DbContext.PulseRateMeasurements.FirstOrDefault(m => m.PulseRate == request.PulseRate && m.ClientDate == request.ClientDate);
            Assert.NotNull(createdPulseRate);
        }

        [Fact(Skip = "Test need currentUserId")]
        public async Task CreateBloodPressureMeasurement_EmptyRequest_ShouldBeValidationError()
        {
            var request = new CreateBloodPressureMeasurementRequestDto();
            await Assert.ThrowsAsync<ValidationApiException>(() => _service.CreateBloodPressureMeasurementAsync(request));
        }

        [Fact(Skip = "Test need currentUserId")]
        public async Task CreateBloodPressureMeasurement_SimpleCall_ShouldReturnCreatedEntityId()
        {
            var pulseRate = 60;
            var now = DateTime.UtcNow;

            var request = new CreateBloodPressureMeasurementRequestDto
            {
                ClientDate = now,
                Systolic = 120,
                Diastolic = 70,
                PulseRate = pulseRate,
            };

            var bloodPressureMeasurementId = await _service.CreateBloodPressureMeasurementAsync(request);

            Assert.NotEqual(Guid.Empty, bloodPressureMeasurementId);

            var createdPulseRate = DbContext.PulseRateMeasurements.FirstOrDefault(m => m.PulseRate == pulseRate && m.ClientDate == now);
            Assert.NotNull(createdPulseRate);
        }

        [Fact(Skip = "Test need currentUserId")]
        public async Task CreatePulseRateMeasurement_EmptyRequest_ShouldBeValidationError()
        {
            var request = new CreatePulseRateMeasurementRequestDto();
            await Assert.ThrowsAsync<ValidationApiException>(() => _service.CreatePulseRateMeasurementAsync(request));
        }

        [Fact(Skip = "Test need currentUserId")]
        public async Task CreatePulseRateMeasurement_SimpleCall_ShouldReturnCreatedEntityId()
        {
            var request = new CreatePulseRateMeasurementRequestDto
            {
                ClientDate = DateTime.Now,
                PulseRate = 60,
            };

            var pulseRateMeasurementId = await _service.CreatePulseRateMeasurementAsync(request);

            Assert.NotEqual(Guid.Empty, pulseRateMeasurementId);
        }

        private CreateSaturationMeasurementRequestDto GetCreateSaturationMeasurementRequestDto(int rawMeasurementsCount)
        {
            return new CreateSaturationMeasurementRequestDto
            {
                Pi = 1,
                SpO2 = 2,
                PulseRate = 3,
                RawMeasurements = Enumerable.Range(1, rawMeasurementsCount)
                    .Select(x => new RawSaturationItemRequestDto
                    {
                        Order = x,
                        Pi = 1,
                        PulseRate = 2,
                        SpO2 = 3,
                        ClientDate = DateTime.UtcNow
                    }).ToList(),
                ClientDate = DateTime.UtcNow
            };
        }
    }
}
