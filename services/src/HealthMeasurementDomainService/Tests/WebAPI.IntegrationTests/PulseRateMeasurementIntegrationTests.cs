using System;
using System.Threading.Tasks;
using Xunit;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.HealthMeasurementDomainService.DAL;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate.Dto;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.IntegrationTests
{
    [Collection("Integration")]
    [Trait("Category", "Integration")]
    public sealed class PulseRateMeasurementIntegrationTests
        : IDisposable, IHttpServiceTests<IPulseRateMeasurementCommandService>, IDbContextTests<HealthMeasurementDomainServiceDbContext>
    {
        public HttpServiceFixture<IPulseRateMeasurementCommandService> HttpServiceFixture { get; }
        public EfCoreContextFixture<HealthMeasurementDomainServiceDbContext> EfCoreContextFixture { get; }
        public HealthMeasurementDomainServiceDbContext DbContext { get; }
        public IPulseRateMeasurementCommandService Service { get; }

        public PulseRateMeasurementIntegrationTests(
            HttpServiceFixture<IPulseRateMeasurementCommandService> httpServiceFixture,
            EfCoreContextFixture<HealthMeasurementDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            DbContext = EfCoreContextFixture.DbContext;
            Service = HttpServiceFixture.GetRestService();
        }

        [Fact]
        public async Task CreatePulseRateMeasurement_SimpleCall_ShouldReturnCreatedEntityId()
        {
            var request = new CreateMeasurementRequestDto<PulseRateMeasurementDto>
            {
                PatientId = Guid.NewGuid(),
                ClientDate = new DateTime(2022, 04, 05, 10, 20, 30),
                Measure = new PulseRateMeasurementDto
                {
                    PulseRate = 60
                }
            };

            var newPulseRateMeasurementId = await Service.CreateAsync(request);
            var newPulseRateMeasurement = await DbContext.PulseRateMeasurements.FindAsync(newPulseRateMeasurementId);

            Assert.NotNull(newPulseRateMeasurement);
            Assert.Equal(newPulseRateMeasurement.Id, newPulseRateMeasurementId);
            Assert.Equal(request.ClientDate, newPulseRateMeasurement.ClientDate);
            Assert.Equal(request.Measure.PulseRate, newPulseRateMeasurement.PulseRate);
            Assert.Equal(request.PatientId, newPulseRateMeasurement.PatientId);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            EfCoreContextFixture.CleanAllTablesExcept().Wait();
        }
    }
}
