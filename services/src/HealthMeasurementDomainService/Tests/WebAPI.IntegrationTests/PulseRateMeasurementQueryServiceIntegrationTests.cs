using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.DAL;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.IntegrationTests
{
    [Trait("Category", "Integration")]
    [Collection("Integration")]
    public sealed class PulseRateMeasurementQueryServiceIntegrationTests
        : IDisposable, IHttpServiceTests<IPulseRateMeasurementQueryService>, IDbContextTests<HealthMeasurementDomainServiceDbContext>
    {
        public HttpServiceFixture<IPulseRateMeasurementQueryService> HttpServiceFixture { get; }
        public EfCoreContextFixture<HealthMeasurementDomainServiceDbContext> EfCoreContextFixture { get; }
        public HealthMeasurementDomainServiceDbContext DbContext { get; }
        public IPulseRateMeasurementQueryService Service { get; }

        public PulseRateMeasurementQueryServiceIntegrationTests(
            HttpServiceFixture<IPulseRateMeasurementQueryService> httpServiceFixture,
            EfCoreContextFixture<HealthMeasurementDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            DbContext = EfCoreContextFixture.DbContext;
            Service = HttpServiceFixture.GetRestService();
        }

        [Fact]
        public async Task GetPulseRateList_EmptyRequest_ShouldBeValidationError()
        {
            var request = new GetMeasurementListRequestDto();
            await Assert.ThrowsAsync<ValidationApiException>(async () => await Service.GetPulseRateList(request));
        }

        [Fact]
        public async Task GetPulseRateList_EmptyRepository_ShouldReturnEmptyMoodList()
        {
            var response = await Service.GetPulseRateList(new GetMeasurementListRequestDto
            {
                Filter = new MeasurementListFilterRequestDto { PatientId = Guid.NewGuid() }
            });

            Assert.NotNull(response);
            Assert.Empty(response.Items);
        }

        [Fact]
        public async Task GetPulseRateList_SimpleCall()
        {
            var patientId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var measurements = new List<PulseRateMeasurement>
            {
                MockPulseRateMeasurement(patientId, now.AddDays(3), 60),
                MockPulseRateMeasurement(patientId, now.AddDays(2), 61),
                MockPulseRateMeasurement(patientId, now.AddDays(1), 62),
                MockPulseRateMeasurement(Guid.NewGuid(), now, 63),
                MockPulseRateMeasurement(patientId, now, 64),
                MockPulseRateMeasurement(patientId, now.AddDays(-1), 65),
                MockPulseRateMeasurement(patientId, now.AddDays(-2), 66),
                MockPulseRateMeasurement(patientId, now.AddDays(-3), 67)
            };
            await DbContext.PulseRateMeasurements.AddRangeAsync(measurements);
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

            var response = await Service.GetPulseRateList(request);

            Assert.NotNull(response);
            Assert.Equal(2, response.Items.Count);
            Assert.NotNull(response.Items.FirstOrDefault(m => m.Measure.PulseRate == 65));
            Assert.NotNull(response.Items.FirstOrDefault(m => m.Measure.PulseRate == 66));
        }

        private static PulseRateMeasurement MockPulseRateMeasurement(Guid patientId, DateTime clientDate,
            int pulseRate)
        {
            return new Fixture().Build<PulseRateMeasurement>()
                .With(_ => _.PatientId, patientId)
                .With(_ => _.PulseRate, pulseRate)
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
