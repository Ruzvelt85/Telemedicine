using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AutoFixture;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsCommandService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsCommandService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.IntegrationTests
{
    [Collection("Integration")]
    [Trait("Category", "Integration")]
    public sealed class CreateOrUpdatePatientIntegrationTests
        : IDisposable, IHttpServiceTests<IPatientsCommandService>, IDbContextTests<HealthCenterStructureDomainServiceDbContext>
    {
        public HttpServiceFixture<IPatientsCommandService> HttpServiceFixture { get; }
        public EfCoreContextFixture<HealthCenterStructureDomainServiceDbContext> EfCoreContextFixture { get; }
        public HealthCenterStructureDomainServiceDbContext DbContext { get; }
        public IPatientsCommandService Service { get; }

        public CreateOrUpdatePatientIntegrationTests(
            HttpServiceFixture<IPatientsCommandService> httpServiceFixture, EfCoreContextFixture<HealthCenterStructureDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            DbContext = EfCoreContextFixture.DbContext;
            Service = HttpServiceFixture.GetRestService();
        }

        [Fact]
        public async Task CreatePatient_IntegrationTest_Successful()
        {
            // Arrange
            var healthCenter1 = Fixtures.GetHealthCenterComposer().Create();
            var healthCenter2 = Fixtures.GetHealthCenterComposer().Create();
            var healthCenterForNewPatient = Fixtures.GetHealthCenterComposer(innerId: "1001_NM").Create();
            var doctorForNewPatient = Fixtures.GetDoctorComposer(innerId: "1995_CO").Create();
            var patient1 = Fixtures.GetPatientComposer(innerId: "16449INV", healthCenterId: healthCenter1.Id).Create();
            var patient2 = Fixtures.GetPatientComposer(innerId: "13377INV", healthCenterId: healthCenter2.Id).Create();

            await DbContext.AddRangeAsync(healthCenter1, healthCenter2, healthCenterForNewPatient);
            await DbContext.AddRangeAsync(doctorForNewPatient);
            await DbContext.AddRangeAsync(patient1, patient2);
            await DbContext.SaveChangesAsync();

            var initialCount = await DbContext.Patients.CountAsync();

            // Act
            var request = new CreateOrUpdatePatientRequestDto
            {
                InnerId = "22266INV",
                LastName = "Lowe",
                FirstName = "Richard",
                PhoneNumber = "3033552515",
                BirthDate = new DateTime(1930, 9, 27),
                HealthCenterInnerId = "1001_NM",
                PrimaryCareProviderInnerId = "1995_CO",
                IsActive = true
            };

            var response = await Service.CreateOrUpdateAsync(request);

            // Assert
            Assert.NotEqual(Guid.Empty, response);
            Assert.Equal(initialCount + 1, await DbContext.Patients.CountAsync());

            var savedPatient = await DbContext.Patients.FindAsync(response);
            Assert.NotNull(savedPatient);
            Assert.Equal("22266INV", savedPatient.InnerId);
            Assert.Equal("Lowe", savedPatient.LastName);
            Assert.Equal("Richard", savedPatient.FirstName);
            Assert.Equal(new DateTime(1930, 9, 27), savedPatient.BirthDate);
            Assert.Equal("3033552515", savedPatient.PhoneNumber);
            Assert.Equal(healthCenterForNewPatient.Id, savedPatient.HealthCenterId);
            Assert.NotNull(savedPatient.PrimaryCareProviderId);
            Assert.Equal(doctorForNewPatient.Id, savedPatient.PrimaryCareProviderId);
            Assert.False(savedPatient.IsDeleted);
        }

        [Fact]
        public async Task UpdatePatient_IntegrationTest_Successful()
        {
            // Arrange
            var healthCenter1 = Fixtures.GetHealthCenterComposer().Create();
            var healthCenterForUpdatingPatient = Fixtures.GetHealthCenterComposer(innerId: "1001_NM").Create();
            var doctorForUpdatingPatient = Fixtures.GetDoctorComposer(innerId: "1995_CO").Create();
            var patient1 = Fixtures.GetPatientComposer(innerId: "16449INV", healthCenterId: healthCenter1.Id).Create();
            var patient2 = Fixtures.GetPatientComposer(innerId: "13377INV", healthCenterId: healthCenterForUpdatingPatient.Id, primaryCareProviderId: doctorForUpdatingPatient.Id).Create();

            await DbContext.AddRangeAsync(healthCenter1, healthCenterForUpdatingPatient);
            await DbContext.AddRangeAsync(doctorForUpdatingPatient);
            await DbContext.AddRangeAsync(patient1, patient2);
            await DbContext.SaveChangesAsync();
            DbContext.Entry(patient2).State = EntityState.Detached;
            var initialCount = await DbContext.Patients.CountAsync();

            // Act
            var request = new CreateOrUpdatePatientRequestDto
            {
                InnerId = "13377INV",
                LastName = "Lowe",
                FirstName = "Richard",
                PhoneNumber = "3033552515",
                BirthDate = new DateTime(1930, 9, 27),
                HealthCenterInnerId = "1001_NM",
                PrimaryCareProviderInnerId = "1995_CO",
                IsActive = true
            };
            var response = await Service.CreateOrUpdateAsync(request);
            await DbContext.SaveChangesAsync();

            // Assert
            Assert.NotEqual(Guid.Empty, response);
            Assert.Equal(initialCount, await DbContext.Patients.CountAsync());

            var updatedPatient = await DbContext.Patients.FindAsync(response);
            Assert.NotNull(updatedPatient);
            Assert.Equal("13377INV", updatedPatient.InnerId);
            Assert.Equal("Lowe", updatedPatient.LastName);
            Assert.Equal("Richard", updatedPatient.FirstName);
            Assert.Equal(new DateTime(1930, 9, 27), updatedPatient.BirthDate);
            Assert.Equal("3033552515", updatedPatient.PhoneNumber);
            Assert.Equal(healthCenterForUpdatingPatient.Id, updatedPatient.HealthCenterId);
            Assert.NotNull(updatedPatient.PrimaryCareProviderId);
            Assert.Equal(doctorForUpdatingPatient.Id, updatedPatient.PrimaryCareProviderId);
            Assert.False(updatedPatient.IsDeleted);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            EfCoreContextFixture.CleanAllTablesExcept().Wait();
        }
    }
}
