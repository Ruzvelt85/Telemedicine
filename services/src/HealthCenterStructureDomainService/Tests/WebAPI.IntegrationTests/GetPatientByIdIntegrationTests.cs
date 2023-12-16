using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;
using Refit;
using Xunit;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.IntegrationTests
{
    [Collection("Integration")]
    [Trait("Category", "Integration")]
    public sealed class GetPatientByIdIntegrationTests
        : IDisposable, IHttpServiceTests<IPatientsQueryService>, IDbContextTests<HealthCenterStructureDomainServiceDbContext>
    {
        public HttpServiceFixture<IPatientsQueryService> HttpServiceFixture { get; }
        public EfCoreContextFixture<HealthCenterStructureDomainServiceDbContext> EfCoreContextFixture { get; }
        public HealthCenterStructureDomainServiceDbContext DbContext { get; }
        public IPatientsQueryService Service { get; }

        public GetPatientByIdIntegrationTests(
            HttpServiceFixture<IPatientsQueryService> httpServiceFixture,
            EfCoreContextFixture<HealthCenterStructureDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            DbContext = EfCoreContextFixture.DbContext;
            Service = HttpServiceFixture.GetRestService();
        }

        [Fact]
        public async Task GetPatientById_ShouldReturnCorrectPatient()
        {
            // Arrange
            var patientId = Guid.NewGuid();

            var healthCenter = Fixtures.GetHealthCenterComposer(Guid.NewGuid(), "Health Center for test", state: "California").Create();
            var careProvider = Fixtures.GetDoctorComposer(Guid.NewGuid(), "Emma", "Watson").Create();
            var patients = new List<Patient>()
            {
                Fixtures.GetPatientComposer(patientId, birthDate: new DateTime(1950, 1, 1), isDeleted: false)
                    .With(p => p.HealthCenter, healthCenter).With(p => p.PrimaryCareProvider, careProvider).Create(),
                Fixtures.GetPatientComposer(Guid.NewGuid(), birthDate: new DateTime(1940, 12, 30), isDeleted: false)
                    .With(p => p.HealthCenter, healthCenter).With(p => p.PrimaryCareProvider, careProvider).Create(),
                Fixtures.GetPatientComposer(Guid.NewGuid(), birthDate: new DateTime(1945, 06, 20), isDeleted: false)
                    .With(_ => _.HealthCenter, healthCenter).With(p => p.PrimaryCareProvider, careProvider).Create(),
                Fixtures.GetPatientComposer(Guid.NewGuid(), isDeleted: false)
                    .With(p => p.HealthCenter, healthCenter).With(p => p.PrimaryCareProvider, careProvider).Create(),
                Fixtures.GetPatientComposer(Guid.NewGuid(), isDeleted: false)
                    .With(p => p.HealthCenter, healthCenter).With(p => p.PrimaryCareProvider, careProvider).Create(),
            };
            await DbContext.AddRangeAsync(patients);
            await DbContext.SaveChangesAsync();

            // Act
            var response = await Service.GetPatientByIdAsync(patientId);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(patientId, response.Id);
            Assert.Equal(patients[0].FirstName, response.FirstName);
            Assert.Equal(patients[0].LastName, response.LastName);
            Assert.Equal(patients[0].PhoneNumber, response.PhoneNumber);
            Assert.Equal(patients[0].BirthDate, response.BirthDate);

            Assert.NotNull(response.HealthCenter);
            Assert.Equal(healthCenter.Id, response.HealthCenter.Id);
            Assert.Equal(healthCenter.Name, response.HealthCenter.Name);
            Assert.Equal(healthCenter.UsaState, response.HealthCenter.UsaState);

            Assert.NotNull(response.PrimaryCareProvider);
            Assert.Equal(careProvider.Id, response.PrimaryCareProvider.Id);
            Assert.Equal(careProvider.FirstName, response.PrimaryCareProvider.FirstName);
            Assert.Equal(careProvider.LastName, response.PrimaryCareProvider.LastName);
        }

        [Fact]
        public async Task GetPatientById_EmptyRepository_ShouldThrowException()
        {
            // Arrange
            var healthCenter = Fixtures.GetHealthCenterComposer().Create();
            var careProvider = Fixtures.GetDoctorComposer().Create();
            var patients = new List<Patient>()
            {
                Fixtures.GetPatientComposer(Guid.NewGuid(), isDeleted: false)
                    .With(p => p.HealthCenter, healthCenter)
                    .With(p => p.PrimaryCareProvider, careProvider).Create()
            };
            await DbContext.AddRangeAsync(patients);
            await DbContext.SaveChangesAsync();

            // Act
            var exception = await Record.ExceptionAsync(() => Service.GetPatientByIdAsync(Guid.NewGuid()));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ApiException>(exception);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            EfCoreContextFixture.CleanAllTablesExcept().Wait();
        }
    }
}
