using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientList;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;
using Xunit;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.IntegrationTests
{
    [Trait("Category", "Integration")]
    [Collection("Integration")]
    public sealed class GetPatientListIntegrationTests
        : IDisposable, IHttpServiceTests<IPatientsQueryService>, IDbContextTests<HealthCenterStructureDomainServiceDbContext>
    {
        public HttpServiceFixture<IPatientsQueryService> HttpServiceFixture { get; }
        public EfCoreContextFixture<HealthCenterStructureDomainServiceDbContext> EfCoreContextFixture { get; }
        public HealthCenterStructureDomainServiceDbContext DbContext { get; }
        public IPatientsQueryService Service { get; }

        public GetPatientListIntegrationTests(
            HttpServiceFixture<IPatientsQueryService> httpServiceFixture,
            EfCoreContextFixture<HealthCenterStructureDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            DbContext = EfCoreContextFixture.DbContext;
            Service = HttpServiceFixture.GetRestService();
        }

        [Fact]
        public async Task FindPatientsSuccessfully_ByInterdisciplinaryTeam_IntegrationTest()
        {
            var (team1Id, team2Id, team3Id, doctorId, healthCenterId) = (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var patientListFilterRequest = new PatientListFilterRequestDto
            {
                Name = "Abc",
                DoctorId = doctorId,
                HealthCenterStructureFilter = HealthCenterStructureFilterType.InterdisciplinaryTeam
            };

            var doctors = new List<Doctor> { Fixtures.GetDoctorComposer(doctorId).Create() };
            var healthCenter = Fixtures.GetHealthCenterComposer(healthCenterId).Create();
            var teamsMock = new List<InterdisciplinaryTeam>()
            {
                Fixtures.GetInterdisciplinaryTeamComposer(team1Id, healthCenterId: healthCenterId, isDeleted: false).With(pc => pc.Doctors, doctors).Create(),
                Fixtures.GetInterdisciplinaryTeamComposer(team2Id, healthCenterId: healthCenterId, isDeleted: true).With(pc => pc.Doctors, doctors).Create(),
                Fixtures.GetInterdisciplinaryTeamComposer(team3Id, healthCenterId: healthCenterId, isDeleted: false).Create()
            };

            var patientsMock = new List<Patient>()
            {
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abce", "Sds", "", "12343a5546", team3Id, isDeleted: false)
                    .With(p => p.PrimaryCareProvider, doctors[0]).With(p => p.HealthCenter, healthCenter).Create(), // teamId doesn't match
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abcd", "Defg", "", "12456789", team1Id, isDeleted: false)
                    .With(p => p.PrimaryCareProvider, doctors[0]).With(p => p.HealthCenter, healthCenter).Create(), // correct (search by Name)
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abch", "Tdfd", "", "1234da25631", team2Id, isDeleted: false)
                    .With(p => p.PrimaryCareProvider, doctors[0]).With(p => p.HealthCenter, healthCenter).Create(), // team is deleted
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abcf", "Dascv", "", "1231dd23563", team1Id, isDeleted: true)
                    .With(p => p.PrimaryCareProvider, doctors[0]).With(p => p.HealthCenter, healthCenter).Create(), // patient is deleted
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Udsfdf", "Abcg", "", "1234256312", team1Id, isDeleted: false)
                    .With(p => p.PrimaryCareProvider, doctors[0]).With(p => p.HealthCenter, healthCenter).Create(), // correct, but skipped because of sorting and paging
            };

            await DbContext.AddRangeAsync(healthCenter);
            await DbContext.AddRangeAsync(teamsMock);
            await DbContext.AddRangeAsync(doctors);
            await DbContext.AddRangeAsync(patientsMock);
            await DbContext.SaveChangesAsync();

            var pagingRequest = new PagingRequestDto(1, 2);
            var request = new PatientListRequestDto
            {
                Filter = patientListFilterRequest,
                Paging = pagingRequest,
                FirstNameSortingType = SortingType.Desc
            };
            var patients = await Service.GetPatientListAsync(request);

            Assert.Single(patients.Items);
            Assert.Equal(3, patients.Paging.Total);
            Assert.Contains(patients.Items, patient => patient.FirstName == "Abcd");
        }

        [Fact]
        public async Task FindPatientsSuccessfully_ByHealthCenter_IntegrationTest()
        {
            var (healthCenter1Id, healthCenter2Id, healthCenter3Id, doctorId) = (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var patientListFilterRequest = new PatientListFilterRequestDto
            {
                Name = "Abc",
                DoctorId = doctorId,
                HealthCenterStructureFilter = HealthCenterStructureFilterType.HealthCenter
            };

            var doctors = new List<Doctor> { Fixtures.GetDoctorComposer(doctorId).Create() };
            var healthCenters = new List<HealthCenter>()
            {
                Fixtures.GetHealthCenterComposer(healthCenter1Id, isDeleted: false).With(pc => pc.Doctors, doctors).Create(),
                Fixtures.GetHealthCenterComposer(healthCenter2Id, isDeleted: true).With(pc => pc.Doctors, doctors).Create(),
                Fixtures.GetHealthCenterComposer(healthCenter3Id, isDeleted: true).Create(),
            };

            var patientsMock = new List<Patient>()
            {
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abce", "Sds", "", "12343a5546", null, healthCenter3Id)
                    .With(p => p.PrimaryCareProvider, doctors[0]).With(p => p.HealthCenter, healthCenters[2]).Create(), // teamId doesn't match
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abcd", "Defg", "", "123456789", null, healthCenter1Id)
                    .With(p => p.PrimaryCareProvider, doctors[0]).With(p => p.HealthCenter, healthCenters[0]).Create(), // correct (search by Name), but skipped because of sorting and paging
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abch", "Tdfd", "", "1234da25631", null, healthCenter2Id)
                    .With(p => p.PrimaryCareProvider, doctors[0]).With(p => p.HealthCenter, healthCenters[1]).Create(), // team is deleted
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abcf", "Dascv", "", "1231dd23563", null, healthCenter1Id, null, true)
                    .With(p => p.PrimaryCareProvider, doctors[0]).With(p => p.HealthCenter, healthCenters[0]).Create(), // patient is deleted
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Udsfdf", "Abcg", "", "1234256312", null, healthCenter1Id)
                    .With(p => p.PrimaryCareProvider, doctors[0]).With(p => p.HealthCenter, healthCenters[0]).Create(), // correct (search by Name)
            };

            await DbContext.AddRangeAsync(healthCenters);
            await DbContext.AddRangeAsync(doctors);
            await DbContext.AddRangeAsync(patientsMock);
            await DbContext.SaveChangesAsync();

            var pagingRequest = new PagingRequestDto(1, 2);
            var request = new PatientListRequestDto
            {
                Paging = pagingRequest,
                Filter = patientListFilterRequest,
                FirstNameSortingType = SortingType.Asc,
            };
            var patients = await Service.GetPatientListAsync(request);

            Assert.Single(patients.Items);
            Assert.Equal(3, patients.Paging.Total);
            Assert.Contains(patients.Items, patient => patient.FirstName == "Udsfdf");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            EfCoreContextFixture.CleanAllTablesExcept().Wait();
        }
    }
}
