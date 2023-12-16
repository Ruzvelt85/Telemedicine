using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoFixture;
using Xunit;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.IntegrationTests
{
    [Trait("Category", "Integration")]
    [Collection("Integration")]
    public sealed class GetDoctorListIntegrationTests
        : IDisposable, IHttpServiceTests<IDoctorsQueryService>, IDbContextTests<HealthCenterStructureDomainServiceDbContext>
    {
        public HttpServiceFixture<IDoctorsQueryService> HttpServiceFixture { get; }
        public EfCoreContextFixture<HealthCenterStructureDomainServiceDbContext> EfCoreContextFixture { get; }
        public HealthCenterStructureDomainServiceDbContext DbContext { get; }
        public IDoctorsQueryService Service { get; }

        public GetDoctorListIntegrationTests(
            HttpServiceFixture<IDoctorsQueryService> httpServiceFixture,
            EfCoreContextFixture<HealthCenterStructureDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            DbContext = EfCoreContextFixture.DbContext;
            Service = HttpServiceFixture.GetRestService();
        }

        [Fact]
        public async Task FindDoctors_Successfully_ByName_IntegrationTest()
        {
            // Arrange
            var (healthCenterId, doctorId) = (Guid.NewGuid(), Guid.NewGuid());
            var healthCenters = new List<HealthCenter>
            {
                Fixtures.GetHealthCenterComposer(healthCenterId).Create()
            };
            var doctors = new List<Doctor>
            {
                Fixtures.GetDoctorComposer(doctorId, firstName: "Ameli", lastName: "Seidu").Create().SetHealthCenters(healthCenters),
                Fixtures.GetDoctorComposer(Guid.NewGuid()).Create()
            };

            await DbContext.AddRangeAsync(healthCenters);
            await DbContext.AddRangeAsync(doctors);
            await DbContext.SaveChangesAsync();

            // Act
            var filterRequest = new DoctorListFilterRequestDto { Name = "Ameli" };
            var request = new DoctorListRequestDto
            {
                Filter = filterRequest,
                Paging = new PagingRequestDto()
            };
            var response = await Service.GetDoctorListAsync(request);

            // Assert
            Assert.Single(response.Items);
            var doctorInResponse = response.Items.ToArray()[0];
            Assert.Equal(doctorId, doctorInResponse.Id);
            Assert.Equal("Ameli", doctorInResponse.FirstName);
            Assert.Equal("Seidu", doctorInResponse.LastName);
            Assert.Single(doctorInResponse.HealthCenters);
            Assert.Equal(healthCenterId, doctorInResponse.HealthCenters.First().Id);
        }

        [Fact]
        public async Task FindDoctors_Successfully_ByHealthCenterAndName_IntegrationTest()
        {
            // Arrange
            var (healthCenterId1, healthCenterId2, healthCenterId3, doctorId) = (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var healthCentersOfFirstDoctor = new List<HealthCenter>
            {
                Fixtures.GetHealthCenterComposer(healthCenterId1).Create(),
                Fixtures.GetHealthCenterComposer(healthCenterId2).Create()
            };
            var healthCentersOfSecondDoctor = new List<HealthCenter>
            {
                Fixtures.GetHealthCenterComposer(healthCenterId3).Create()
            };
            var doctors = new List<Doctor>()
            {
                Fixtures.GetDoctorComposer(Guid.NewGuid(), firstName: "Ameli").Create().SetHealthCenters(healthCentersOfFirstDoctor),
                Fixtures.GetDoctorComposer(doctorId, firstName: "Laura").Create().SetHealthCenters(healthCentersOfSecondDoctor),
                Fixtures.GetDoctorComposer(Guid.NewGuid()).Create()
            };

            await DbContext.AddRangeAsync(healthCentersOfFirstDoctor);
            await DbContext.AddRangeAsync(healthCentersOfSecondDoctor);
            await DbContext.AddRangeAsync(doctors);
            await DbContext.SaveChangesAsync();

            // Act
            var filterRequest = new DoctorListFilterRequestDto { Name = "Laura", HealthCenterIds = new[] { healthCenterId3 } };
            var request = new DoctorListRequestDto
            {
                Filter = filterRequest,
                Paging = new PagingRequestDto()
            };
            var response = await Service.GetDoctorListAsync(request);

            // Assert
            Assert.Single(response.Items);
            var doctorInResponse = response.Items.ToArray()[0];
            Assert.Equal(doctorId, doctorInResponse.Id);
            Assert.Equal("Laura", doctorInResponse.FirstName);
            Assert.Single(doctorInResponse.HealthCenters);
            Assert.Equal(healthCenterId3, doctorInResponse.HealthCenters.First().Id);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            EfCoreContextFixture.CleanAllTablesExcept().Wait();
        }
    }
}
