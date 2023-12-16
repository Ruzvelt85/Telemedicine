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
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.IntegrationTests
{
    [Trait("Category", "Integration")]
    [Collection("Integration")]
    public sealed class BloodPressureMeasurementQueryServiceIntegrationTests
        : IDisposable, IHttpServiceTests<IBloodPressureMeasurementQueryService>, IDbContextTests<HealthMeasurementDomainServiceDbContext>
    {
        public HttpServiceFixture<IBloodPressureMeasurementQueryService> HttpServiceFixture { get; }
        public EfCoreContextFixture<HealthMeasurementDomainServiceDbContext> EfCoreContextFixture { get; }
        public HealthMeasurementDomainServiceDbContext DbContext { get; }
        public IBloodPressureMeasurementQueryService Service { get; }

        public BloodPressureMeasurementQueryServiceIntegrationTests(
            HttpServiceFixture<IBloodPressureMeasurementQueryService> httpServiceFixture,
            EfCoreContextFixture<HealthMeasurementDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            DbContext = EfCoreContextFixture.DbContext;
            Service = HttpServiceFixture.GetRestService();
        }

        [Fact]
        public async Task GetBloodPressureList_EmptyRequest_ShouldBeValidationError()
        {
            var request = new GetMeasurementListRequestDto();
            await Assert.ThrowsAsync<ValidationApiException>(async () => await Service.GetBloodPressureList(request));
        }

        [Fact]
        public async Task GetBloodPressureList_EmptyRepository_ShouldReturnEmptyMoodList()
        {
            var response = await Service.GetBloodPressureList(new GetMeasurementListRequestDto
            {
                Filter = new MeasurementListFilterRequestDto { PatientId = Guid.NewGuid() }
            });

            Assert.NotNull(response);
            Assert.Empty(response.Items);
        }

        [Fact]
        public async Task GetBloodPressureList_SimpleCall()
        {
            var patientId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var measurements = new List<BloodPressureMeasurement>
            {
                MockBloodPressureMeasurement(patientId, now.AddDays(3), 120, 70, 60),
                MockBloodPressureMeasurement(patientId, now.AddDays(2), 121, 71, 61),
                MockBloodPressureMeasurement(patientId, now.AddDays(1), 122, 72, 62),
                MockBloodPressureMeasurement(Guid.NewGuid(), now, 123, 73, 63),
                MockBloodPressureMeasurement(patientId, now, 124, 74, 64),
                MockBloodPressureMeasurement(patientId, now.AddDays(-1), 125, 75, 65),
                MockBloodPressureMeasurement(patientId, now.AddDays(-2), 126, 76, 66),
                MockBloodPressureMeasurement(patientId, now.AddDays(-3), 127, 77, 67)
            };
            await DbContext.BloodPressureMeasurements.AddRangeAsync(measurements);
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

            var response = await Service.GetBloodPressureList(request);

            Assert.NotNull(response);
            Assert.Equal(2, response.Items.Count);
            Assert.NotNull(response.Items.FirstOrDefault(m => m.Measure.Systolic == 125 && m.Measure.Diastolic == 75 && m.Measure.PulseRate == 65));
            Assert.NotNull(response.Items.FirstOrDefault(m => m.Measure.Systolic == 126 && m.Measure.Diastolic == 76 && m.Measure.PulseRate == 66));
        }

        private static BloodPressureMeasurement MockBloodPressureMeasurement(Guid patientId, DateTime clientDate,
            int systolic, int diastolic, int pulseRate)
        {
            return new Fixture().Build<BloodPressureMeasurement>()
                .With(_ => _.PatientId, patientId)
                .With(_ => _.Systolic, systolic)
                .With(_ => _.Diastolic, diastolic)
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
