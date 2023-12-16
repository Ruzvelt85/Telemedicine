using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Xunit;
using Refit;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.DAL;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.IntegrationTests
{
    [Trait("Category", "Integration")]
    [Collection("Integration")]
    public sealed class MoodMeasurementQueryServiceIntegrationTests
        : IDisposable, IHttpServiceTests<IMoodMeasurementQueryService>, IDbContextTests<HealthMeasurementDomainServiceDbContext>
    {
        public HttpServiceFixture<IMoodMeasurementQueryService> HttpServiceFixture { get; }
        public EfCoreContextFixture<HealthMeasurementDomainServiceDbContext> EfCoreContextFixture { get; }
        public HealthMeasurementDomainServiceDbContext DbContext { get; }
        public IMoodMeasurementQueryService Service { get; }

        public MoodMeasurementQueryServiceIntegrationTests(
            HttpServiceFixture<IMoodMeasurementQueryService> httpServiceFixture,
            EfCoreContextFixture<HealthMeasurementDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            DbContext = EfCoreContextFixture.DbContext;
            Service = HttpServiceFixture.GetRestService();
        }

        [Fact]
        public async Task GetMoodList_EmptyRequest_ShouldBeValidationError()
        {
            var request = new GetMeasurementListRequestDto();
            await Assert.ThrowsAsync<ValidationApiException>(async () => await Service.GetMoodList(request));
        }

        [Fact]
        public async Task GetMoodList_EmptyRepository_ShouldReturnEmptyMoodList()
        {
            var response = await Service.GetMoodList(new GetMeasurementListRequestDto
            {
                Filter = new MeasurementListFilterRequestDto { PatientId = Guid.NewGuid() }
            });

            Assert.NotNull(response);
            Assert.Empty(response.Items);
        }

        [Fact]
        public async Task GetMoodList_SimpleCall()
        {
            var patientId = Guid.NewGuid();
            var now = DateTime.Now;

            var measurements = new List<MoodMeasurement>
            {
                MockMoodMeasurement(patientId, now.AddDays(3), MoodMeasureType.Default),
                MockMoodMeasurement(patientId, now.AddDays(2), MoodMeasureType.Happy),
                MockMoodMeasurement(patientId, now.AddDays(1), MoodMeasureType.Sad),
                MockMoodMeasurement(Guid.NewGuid(), now, MoodMeasureType.Default),
                MockMoodMeasurement(patientId, now, MoodMeasureType.Unhappy),
                MockMoodMeasurement(patientId, now.AddDays(-1), MoodMeasureType.Satisfied),
                MockMoodMeasurement(patientId, now.AddDays(-2), MoodMeasureType.Neutral),
                MockMoodMeasurement(patientId, now.AddDays(-3), MoodMeasureType.Default)
            };
            await DbContext.MoodMeasurements.AddRangeAsync(measurements);
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

            var response = await Service.GetMoodList(request);

            Assert.NotNull(response);
            Assert.Equal(2, response.Items.Count);
            Assert.NotNull(response.Items.FirstOrDefault(m => m.Measure.Measure == API.Common.Mood.Dto.MoodMeasureType.Satisfied));
            Assert.NotNull(response.Items.FirstOrDefault(m => m.Measure.Measure == API.Common.Mood.Dto.MoodMeasureType.Neutral));
        }

        private static MoodMeasurement MockMoodMeasurement(Guid patientId, DateTime clientDate, MoodMeasureType measure)
        {
            return new Fixture().Build<MoodMeasurement>()
                .With(_ => _.PatientId, patientId)
                .With(_ => _.Measure, measure)
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
