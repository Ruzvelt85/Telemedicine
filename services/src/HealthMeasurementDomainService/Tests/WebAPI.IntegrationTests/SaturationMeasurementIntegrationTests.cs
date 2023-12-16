using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.HealthMeasurementDomainService.DAL;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.IntegrationTests
{
    [Trait("Category", "Integration")]
    [Collection("Integration")]
    public sealed class SaturationMeasurementIntegrationTests
        : IDisposable, IHttpServiceTests<ISaturationMeasurementCommandService>, IDbContextTests<HealthMeasurementDomainServiceDbContext>
    {
        public HttpServiceFixture<ISaturationMeasurementCommandService> HttpServiceFixture { get; }
        public EfCoreContextFixture<HealthMeasurementDomainServiceDbContext> EfCoreContextFixture { get; }
        public HealthMeasurementDomainServiceDbContext DbContext { get; }
        public ISaturationMeasurementCommandService Service { get; }

        public SaturationMeasurementIntegrationTests(
            HttpServiceFixture<ISaturationMeasurementCommandService> httpServiceFixture,
            EfCoreContextFixture<HealthMeasurementDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            DbContext = EfCoreContextFixture.DbContext;
            Service = HttpServiceFixture.GetRestService();
        }

        [Fact]
        public async Task CreateSaturationMeasurement_SimpleCall_ShouldReturnCreatedEntityId()
        {
            // Arrange
            var request = new CreateMeasurementRequestDto<SaturationMeasurementDto>
            {
                PatientId = Guid.NewGuid(),
                ClientDate = new DateTime(2022, 04, 05, 10, 20, 30),
                Measure = new SaturationMeasurementDto
                {
                    PulseRate = 1,
                    Pi = 2,
                    SpO2 = 3,
                    RawMeasurements = null
                }
            };

            // Act
            var newSaturationMeasurementId = await Service.CreateAsync(request);

            // Assert
            var newSaturationMeasurement = await DbContext.SaturationMeasurements.Include(x => x.RawSaturationData)
                .FirstOrDefaultAsync(x => x.Id == newSaturationMeasurementId);

            Assert.NotNull(newSaturationMeasurement);
            Assert.Equal(newSaturationMeasurement.Id, newSaturationMeasurementId);
            Assert.Equal(request.PatientId, newSaturationMeasurement.PatientId);
            Assert.Equal(request.Measure.PulseRate, newSaturationMeasurement.PulseRate);
            Assert.Equal(request.Measure.Pi, newSaturationMeasurement.Pi);
            Assert.Equal(request.Measure.SpO2, newSaturationMeasurement.SpO2);
            Assert.Null(newSaturationMeasurement.RawSaturationData);
            Assert.Equal(newSaturationMeasurement.Id, newSaturationMeasurementId);
            Assert.Equal(request.ClientDate, newSaturationMeasurement.ClientDate);
        }

        [Fact]
        public async Task CreateSaturationMeasurement_WithRawMeasurement_ShouldReturnCreatedEntityId()
        {
            // Arrange
            var request = new CreateMeasurementRequestDto<SaturationMeasurementDto>
            {
                PatientId = Guid.NewGuid(),
                ClientDate = new DateTime(2022, 04, 05, 10, 20, 30),
                Measure = new SaturationMeasurementDto
                {
                    PulseRate = 1,
                    Pi = 2,
                    SpO2 = 3,
                    RawMeasurements = new List<RawSaturationMeasurementItemDto>
                    {
                        new()
                        {
                            Order = 1,
                            PulseRate = 2,
                            Pi = 3,
                            SpO2 = 4,
                            ClientDate = DateTime.Now
                        },
                        new()
                        {
                            Order = 2,
                            PulseRate = 3,
                            Pi = 4,
                            SpO2 = 5,
                            ClientDate = DateTime.Now
                        }
                    }
                }
            };

            // Act
            var newSaturationMeasurementId = await Service.CreateAsync(request);

            // Assert
            var newSaturationMeasurement = await DbContext.SaturationMeasurements
                .Include(x => x.RawSaturationData)
                .FirstOrDefaultAsync(x => x.Id == newSaturationMeasurementId);

            Assert.NotNull(newSaturationMeasurement);
            Assert.Equal(newSaturationMeasurement.Id, newSaturationMeasurementId);
            Assert.Equal(request.PatientId, newSaturationMeasurement.PatientId);
            Assert.Equal(request.Measure.PulseRate, newSaturationMeasurement.PulseRate);
            Assert.Equal(request.Measure.Pi, newSaturationMeasurement.Pi);
            Assert.Equal(request.Measure.SpO2, newSaturationMeasurement.SpO2);
            Assert.NotEqual(Guid.Empty, newSaturationMeasurement.RawSaturationData!.Id);
            Assert.Equal(newSaturationMeasurement.Id, newSaturationMeasurement.RawSaturationData!.Id);
            Assert.Equal(request.Measure.RawMeasurements!.Count, newSaturationMeasurement.RawSaturationData.Items.Count);
            Assert.Equal(request.ClientDate, newSaturationMeasurement.ClientDate);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            EfCoreContextFixture.CleanAllTablesExcept().Wait();
        }
    }
}
