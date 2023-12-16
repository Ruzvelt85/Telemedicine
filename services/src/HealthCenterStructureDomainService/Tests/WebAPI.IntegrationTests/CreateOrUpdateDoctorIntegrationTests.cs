using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Refit;
using System;
using System.Threading.Tasks;
using Xunit;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsCommandService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsCommandService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Enums;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.IntegrationTests
{
    [Collection("Integration")]
    [Trait("Category", "Integration")]
    public sealed class CreateOrUpdateDoctorIntegrationTests : IDisposable, IHttpServiceTests<IDoctorsCommandService>, IDbContextTests<HealthCenterStructureDomainServiceDbContext>
    {
        /// <inheritdoc />
        public HttpServiceFixture<IDoctorsCommandService> HttpServiceFixture { get; }

        /// <inheritdoc />
        public EfCoreContextFixture<HealthCenterStructureDomainServiceDbContext> EfCoreContextFixture { get; }

        public IDoctorsCommandService Service { get; }

        public HealthCenterStructureDomainServiceDbContext DbContext { get; }

        public CreateOrUpdateDoctorIntegrationTests(
            HttpServiceFixture<IDoctorsCommandService> httpServiceFixture,
            EfCoreContextFixture<HealthCenterStructureDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            Service = HttpServiceFixture.GetRestService();
            DbContext = EfCoreContextFixture.DbContext;
        }

        [Fact]
        public async Task CreateOrUpdateDoctor_DefaultRequest_ShouldThrowValidationApiException()
        {
            var request = new CreateOrUpdateDoctorRequestDto();
            var exception = await Record.ExceptionAsync(() => Service.CreateOrUpdateAsync(request));

            Assert.NotNull(exception);
            Assert.IsType<ValidationApiException>(exception);
        }

        [Fact]
        public async Task CreateOrUpdateDoctor_NewUser_ShouldCreateDoctor()
        {
            var doctor = Fixtures.GetDoctorComposer(innerId: "doctor inner id",
               firstName: "first name", lastName: "last name").Create();
            await DbContext.Doctors.AddAsync(doctor);

            var testHealthCenter = Fixtures.GetHealthCenterComposer(innerId: "test health center inner id",
                name: "health center name", state: "NY").Create();
            await DbContext.HealthCenters.AddAsync(testHealthCenter);
            await DbContext.SaveChangesAsync();

            DbContext.ChangeTracker.Clear();

            var request = new CreateOrUpdateDoctorRequestDto
            {
                InnerId = doctor.InnerId,
                FirstName = "fn changed",
                LastName = "ln changed",
                HealthCenterInnerIds = new[] { testHealthCenter.InnerId },
                IsActive = true
            };

            // Act
            var result = await Service.CreateOrUpdateAsync(request);
            await DbContext.SaveChangesAsync();

            //Assert
            var updatedDoctor = await DbContext.Doctors.Include(doctor => doctor.HealthCenters)
                .FirstOrDefaultAsync(_ => _.InnerId == doctor.InnerId);
            Assert.NotNull(updatedDoctor);
            Assert.Equal(updatedDoctor.Id, result);
            Assert.Equal(request.FirstName, updatedDoctor.FirstName);
            Assert.Equal(request.LastName, updatedDoctor.LastName);
            Assert.Equal(request.InnerId, updatedDoctor.InnerId);
            Assert.Equal(UserType.Doctor, updatedDoctor.Type);
            Assert.NotNull(updatedDoctor.HealthCenters!);
            Assert.Equal(1, updatedDoctor.HealthCenters!.Count);

            foreach (HealthCenter doctorHealthCenter in updatedDoctor.HealthCenters)
            {
                Assert.Equal(testHealthCenter.InnerId, doctorHealthCenter.InnerId);
                Assert.Equal(testHealthCenter.Name, doctorHealthCenter.Name);
                Assert.Equal(testHealthCenter.UsaState, doctorHealthCenter.UsaState);
            }
        }

        [Fact]
        public async Task CreateOrUpdateDoctor_NewUserAndChangeHealthCenter_ShouldCreateDoctor()
        {
            var initialHealthCenter = Fixtures.GetHealthCenterComposer(innerId: "initial health center inner id",
                name: "initial health center name", state: "NY").Create();
            await DbContext.HealthCenters.AddAsync(initialHealthCenter);

            var doctor = Fixtures.GetDoctorComposer(innerId: "doctor inner id",
                firstName: "first name", lastName: "last name").Create();
            doctor.SetHealthCenters(new[] { initialHealthCenter });

            var newHealthCenter = Fixtures.GetHealthCenterComposer(innerId: "new health center inner id",
                name: "new health center name", state: "NY").Create();
            await DbContext.HealthCenters.AddAsync(newHealthCenter);

            await DbContext.Doctors.AddAsync(doctor);
            await DbContext.SaveChangesAsync();

            DbContext.ChangeTracker.Clear();

            var request = new CreateOrUpdateDoctorRequestDto
            {
                InnerId = doctor.InnerId,
                FirstName = "fn changed",
                LastName = "ln changed",
                HealthCenterInnerIds = new[] { newHealthCenter.InnerId },
                IsActive = true
            };

            // Act
            var result = await Service.CreateOrUpdateAsync(request);
            await DbContext.SaveChangesAsync();

            //Assert
            var updatedDoctor = await DbContext.Doctors.Include(doctor => doctor.HealthCenters)
                .FirstOrDefaultAsync(_ => _.InnerId == doctor.InnerId);
            Assert.NotNull(updatedDoctor);
            Assert.Equal(updatedDoctor.Id, result);
            Assert.Equal(request.FirstName, updatedDoctor.FirstName);
            Assert.Equal(request.LastName, updatedDoctor.LastName);
            Assert.Equal(request.InnerId, updatedDoctor.InnerId);
            Assert.Equal(UserType.Doctor, updatedDoctor.Type);
            Assert.NotNull(updatedDoctor.HealthCenters!);
            Assert.Equal(1, updatedDoctor.HealthCenters!.Count);

            foreach (HealthCenter doctorHealthCenter in updatedDoctor.HealthCenters)
            {
                Assert.Equal(newHealthCenter.InnerId, doctorHealthCenter.InnerId);
                Assert.Equal(newHealthCenter.Name, doctorHealthCenter.Name);
                Assert.Equal(newHealthCenter.UsaState, doctorHealthCenter.UsaState);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            EfCoreContextFixture.CleanAllTablesExcept().Wait();
        }
    }
}
