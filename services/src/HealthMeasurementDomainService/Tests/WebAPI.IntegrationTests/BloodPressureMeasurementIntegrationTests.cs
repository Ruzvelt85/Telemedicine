using System;
using System.Threading.Tasks;
using Xunit;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.HealthMeasurementDomainService.DAL;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure.Dto;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.IntegrationTests
{
    [Collection("Integration")]
    [Trait("Category", "Integration")]
    public sealed class BloodPressureMeasurementIntegrationTests
        : IDisposable, IHttpServiceTests<IBloodPressureMeasurementCommandService>, IDbContextTests<HealthMeasurementDomainServiceDbContext>
    {
        public HttpServiceFixture<IBloodPressureMeasurementCommandService> HttpServiceFixture { get; }
        public EfCoreContextFixture<HealthMeasurementDomainServiceDbContext> EfCoreContextFixture { get; }
        public HealthMeasurementDomainServiceDbContext DbContext { get; }
        public IBloodPressureMeasurementCommandService Service { get; }

        public BloodPressureMeasurementIntegrationTests(
            HttpServiceFixture<IBloodPressureMeasurementCommandService> httpServiceFixture,
            EfCoreContextFixture<HealthMeasurementDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            DbContext = EfCoreContextFixture.DbContext;
            Service = HttpServiceFixture.GetRestService();
        }

        [Fact]
        public async Task CreateBloodPressureMeasurement_SimpleCall_ShouldReturnCreatedEntityId()
        {
            var request = new CreateMeasurementRequestDto<BloodPressureMeasurementDto>
            {
                PatientId = Guid.NewGuid(),
                ClientDate = new DateTime(2022, 04, 05, 10, 20, 30),
                Measure = new BloodPressureMeasurementDto
                {
                    Systolic = 120,
                    Diastolic = 70,
                    PulseRate = 60
                }
            };

            var newBloodPressureMeasurementId = await Service.CreateAsync(request);
            var newBloodPressureMeasurement = await DbContext.BloodPressureMeasurements.FindAsync(newBloodPressureMeasurementId);

            Assert.NotNull(newBloodPressureMeasurement);
            Assert.Equal(newBloodPressureMeasurement.Id, newBloodPressureMeasurementId);
            Assert.Equal(request.ClientDate, newBloodPressureMeasurement.ClientDate);
            Assert.Equal(request.Measure.Systolic, newBloodPressureMeasurement.Systolic);
            Assert.Equal(request.Measure.Diastolic, newBloodPressureMeasurement.Diastolic);
            Assert.Equal(request.Measure.PulseRate, newBloodPressureMeasurement.PulseRate);
            Assert.Equal(request.PatientId, newBloodPressureMeasurement.PatientId);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            EfCoreContextFixture.CleanAllTablesExcept().Wait();
        }
    }
}
