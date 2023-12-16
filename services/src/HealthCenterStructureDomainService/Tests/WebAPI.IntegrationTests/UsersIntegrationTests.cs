using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;
using Refit;
using Xunit;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.IntegrationTests
{
    [Collection("Integration")]
    [Trait("Category", "Integration")]
    public sealed class UsersIntegrationTests
        : IDisposable, IHttpServiceTests<IUsersQueryService>, IDbContextTests<HealthCenterStructureDomainServiceDbContext>
    {
        public HttpServiceFixture<IUsersQueryService> HttpServiceFixture { get; }
        public EfCoreContextFixture<HealthCenterStructureDomainServiceDbContext> EfCoreContextFixture { get; }
        public HealthCenterStructureDomainServiceDbContext DbContext { get; }
        public IUsersQueryService Service { get; }

        public UsersIntegrationTests(
            HttpServiceFixture<IUsersQueryService> httpServiceFixture, EfCoreContextFixture<HealthCenterStructureDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            DbContext = EfCoreContextFixture.DbContext;
            Service = HttpServiceFixture.GetRestService();
        }

        [Fact]
        public async Task GetUserInfo_IntegrationTest_Successful()
        {
            var patientId = Guid.NewGuid();
            var healthCenter = Fixtures.GetHealthCenterComposer().Create();
            var careProvider = Fixtures.GetDoctorComposer().Create();
            var patient = Fixtures.GetPatientComposer(patientId)
                .With(p => p.HealthCenter, healthCenter)
                .With(p => p.PrimaryCareProvider, careProvider).Create();
            var patientsMock = new List<Patient>
            {
                Fixtures.GetPatientComposer(Guid.NewGuid())
                    .With(p => p.HealthCenter, healthCenter)
                    .With(p => p.PrimaryCareProvider, careProvider).Create(),
                patient,
                Fixtures.GetPatientComposer(Guid.NewGuid())
                    .With(p => p.HealthCenter, healthCenter)
                    .With(p => p.PrimaryCareProvider, careProvider).Create(),
            };

            await DbContext.AddRangeAsync(patientsMock);
            await DbContext.SaveChangesAsync();

            var response = await Service.GetUserInfoAsync(patientId);

            Assert.NotNull(response);
            Assert.Equal(patientId, response.Id);
            Assert.Equal(UserType.Patient, response.Type);
        }

        [Fact]
        public async Task GetUserInfo_IntegrationTest_NotFound()
        {
            var doctorId = Guid.NewGuid();
            var doctorsMock = new List<Doctor>
            {
                Fixtures.GetDoctorComposer(Guid.NewGuid()).Create(),
                Fixtures.GetDoctorComposer(Guid.NewGuid()).Create(),
                Fixtures.GetDoctorComposer(Guid.NewGuid()).Create()
            };

            await DbContext.AddRangeAsync(doctorsMock);
            await DbContext.SaveChangesAsync();

            var exception = await Record.ExceptionAsync(() => Service.GetUserInfoAsync(doctorId));

            Assert.NotNull(exception);
            Assert.IsType<ApiException>(exception);
        }

        [Fact]
        public async Task GetUserInfoDetails_PatientInfo_IntegrationTest()
        {
            var patientId = Guid.NewGuid();
            var healthCenter = Fixtures.GetHealthCenterComposer().Create();
            var careProvider = Fixtures.GetDoctorComposer().Create();
            var patient = Fixtures.GetPatientComposer(patientId, birthDate: new DateTime(1950, 1, 1))
                .With(p => p.HealthCenter, healthCenter)
                .With(p => p.PrimaryCareProvider, careProvider).Create();
            var patientsMock = new List<Patient>
            {
                Fixtures.GetPatientComposer(Guid.NewGuid())
                    .With(p => p.HealthCenter, healthCenter)
                    .With(p => p.PrimaryCareProvider, careProvider).Create(),
                patient,
                Fixtures.GetPatientComposer(Guid.NewGuid())
                    .With(p => p.HealthCenter, healthCenter)
                    .With(p => p.PrimaryCareProvider, careProvider).Create(),
            };

            await DbContext.AddRangeAsync(patientsMock);
            await DbContext.SaveChangesAsync();

            var response = await Service.GetUserInfoDetailsAsync(patientId);

            Assert.NotNull(response);
            Assert.Equal(patientId, response.Id);
            Assert.Equal(UserType.Patient, response.Type);
            Assert.IsType<PatientInfoResponseDto>(response);

            var patientInfo = response as PatientInfoResponseDto;
            Assert.NotNull(patientInfo);
            Assert.Equal(patientInfo.BirthDate, patient.BirthDate);
        }

        [Fact]
        public async Task GetUserInfoDetails_DoctorInfo_IntegrationTest()
        {
            var doctorId = Guid.NewGuid();
            var doctor = Fixtures.GetDoctorComposer(doctorId).Create();
            var doctorsMock = new List<Doctor>
            {
                Fixtures.GetDoctorComposer(Guid.NewGuid()).Create(),
                doctor,
                Fixtures.GetDoctorComposer(Guid.NewGuid()).Create()
            };

            await DbContext.AddRangeAsync(doctorsMock);
            await DbContext.SaveChangesAsync();

            var response = await Service.GetUserInfoDetailsAsync(doctorId);

            Assert.NotNull(response);
            Assert.Equal(doctorId, response.Id);
            Assert.Equal(UserType.Doctor, response.Type);
            Assert.IsType<DoctorInfoResponseDto>(response);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            EfCoreContextFixture.CleanAllTablesExcept().Wait();
        }
    }
}
