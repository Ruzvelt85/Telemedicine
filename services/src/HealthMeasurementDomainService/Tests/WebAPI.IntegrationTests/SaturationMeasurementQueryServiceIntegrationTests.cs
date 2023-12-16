using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.DAL;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.IntegrationTests
{
    [Trait("Category", "Integration")]
    [Collection("Integration")]
    public sealed class SaturationMeasurementQueryServiceIntegrationTests
        : IDisposable, IHttpServiceTests<ISaturationMeasurementQueryService>, IDbContextTests<HealthMeasurementDomainServiceDbContext>
    {
        public HttpServiceFixture<ISaturationMeasurementQueryService> HttpServiceFixture { get; }
        public EfCoreContextFixture<HealthMeasurementDomainServiceDbContext> EfCoreContextFixture { get; }
        public HealthMeasurementDomainServiceDbContext DbContext { get; }
        public ISaturationMeasurementQueryService Service { get; }

        public SaturationMeasurementQueryServiceIntegrationTests(
            HttpServiceFixture<ISaturationMeasurementQueryService> httpServiceFixture,
            EfCoreContextFixture<HealthMeasurementDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            DbContext = EfCoreContextFixture.DbContext;
            Service = HttpServiceFixture.GetRestService();
        }

        [Fact]
        public async Task GetSaturationMeasurementList_EmptyRequest_ShouldBeValidationError()
        {
            var request = new GetMeasurementListRequestDto();
            await Assert.ThrowsAsync<ValidationApiException>(async () => await Service.GetSaturationList(request));
        }

        [Fact]
        public async Task GetSaturationMeasurementList_EmptyRepository_ShouldReturnEmptyList()
        {
            var response = await Service.GetSaturationList(new GetMeasurementListRequestDto
            {
                Filter = new MeasurementListFilterRequestDto { PatientId = Guid.NewGuid() }
            });

            Assert.NotNull(response);
            Assert.Empty(response.Items);
        }

        [Fact]
        public async Task GetSaturationMeasurementList_SimpleCall()
        {
            var patientId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var measurements = new List<SaturationMeasurement>
            {
                MockSaturationMeasurement(patientId, now.AddDays(3), 50),
                MockSaturationMeasurement(patientId, now.AddDays(2), 90),
                MockSaturationMeasurement(patientId, now.AddDays(1), 0),
                MockSaturationMeasurement(Guid.NewGuid(), now, 60),
                MockSaturationMeasurement(patientId, now, 10),
                MockSaturationMeasurement(patientId, now.AddDays(-1), 20),
                MockSaturationMeasurement(patientId, now.AddDays(-2), 40),
                MockSaturationMeasurement(patientId, now.AddDays(-3), 80)
            };
            await DbContext.SaturationMeasurements.AddRangeAsync(measurements);
            await DbContext.SaveChangesAsync();

            var request = new GetMeasurementListRequestDto
            {
                Paging = new PagingRequestDto(2, 2),
                Filter = new MeasurementListFilterRequestDto
                {
                    PatientId = patientId,
                    DateRange = new Range<DateTime?>(now.AddDays(-2), now.AddDays(2))
                }
            };

            var response = await Service.GetSaturationList(request);

            Assert.NotNull(response);
            Assert.Equal(2, response.Items.Count);
            Assert.NotNull(response.Items.FirstOrDefault(m => m.Measure.SpO2 == 20));
            Assert.NotNull(response.Items.FirstOrDefault(m => m.Measure.SpO2 == 40));
        }

        private static SaturationMeasurement MockSaturationMeasurement(Guid patientId, DateTime clientDate, int oxygenSaturation)
        {
            return new Fixture().Build<SaturationMeasurement>()
                .With(_ => _.PatientId, patientId)
                .With(_ => _.SpO2, oxygenSaturation)
                .With(_ => _.ClientDate, clientDate)
                .Create();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            EfCoreContextFixture.CleanAllTablesExcept().Wait();
        }
    }
}
