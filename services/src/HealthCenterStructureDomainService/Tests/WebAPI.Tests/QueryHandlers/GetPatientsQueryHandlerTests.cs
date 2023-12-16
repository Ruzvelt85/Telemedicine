using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientList;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.InterdisciplinaryTeam;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.HealthCenter;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.User;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.QueryHandlers
{
    public class GetPatientsQueryHandlerTests
    {
        private readonly GetPatientsQueryHandler _queryHandler;

        private readonly PagingRequestDto _defaultPagingRequest = new()
        {
            Take = 10,
            Skip = 0
        };

        private readonly HealthCenterStructureDomainServiceDbContext _context;

        public GetPatientsQueryHandlerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<HealthCenterStructureDomainServiceDbContext>()
                .UseInMemoryDatabase($"GetPatientsQueryHandlerTests-{Guid.NewGuid()}");

            _context = new HealthCenterStructureDomainServiceDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());

            var patientRepository = new PatientReadRepository(_context);
            var idtRepository = new InterdisciplinaryTeamReadRepository(_context);
            var healthCenterRepository = new HealthCenterReadRepository(_context);

            IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();

            _queryHandler = new GetPatientsQueryHandler(mapper, patientRepository, idtRepository, healthCenterRepository);
        }

        #region GetInterdisciplinaryTeamIdsByDoctorId

        [Fact]
        public async Task EmptyTeamsRepository_Test()
        {
            var doctorId = Guid.NewGuid();

            var ids = await _queryHandler.GetInterdisciplinaryTeamIdsByDoctorId(doctorId);

            Assert.Empty(ids);
        }

        [Fact]
        public async Task FindTeam_WithAllFilters_Successfully_Test()
        {
            var (doctorId, teamId) = (Guid.NewGuid(), Guid.NewGuid());
            var doctors = new List<Doctor> { Fixtures.GetDoctorComposer(doctorId).Create() };
            var teamsMock = new List<InterdisciplinaryTeam>
            {
                Fixtures.GetInterdisciplinaryTeamComposer(teamId)
                    .With(pc => pc.Doctors, doctors).Create()
            };
            await _context.AddRangeAsync(teamsMock);
            await _context.SaveChangesAsync();

            var ids = (await _queryHandler.GetInterdisciplinaryTeamIdsByDoctorId(doctorId)).ToList();

            Assert.Single(ids);
            Assert.Equal(teamId, ids.FirstOrDefault());
        }

        [Fact]
        public async Task SkipDeletedTeam_Test()
        {
            var (doctorId, teamId) = (Guid.NewGuid(), Guid.NewGuid());
            var doctors = new List<Doctor> { Fixtures.GetDoctorComposer(doctorId).Create() };
            var teamsMock = new List<InterdisciplinaryTeam>
            {
                Fixtures.GetInterdisciplinaryTeamComposer(teamId, isDeleted: true)
                    .With(pc => pc.Doctors, doctors).Create()
            };
            await _context.AddRangeAsync(teamsMock);
            await _context.SaveChangesAsync();

            var ids = await _queryHandler.GetInterdisciplinaryTeamIdsByDoctorId(doctorId);

            Assert.Empty(ids);
        }

        [Fact]
        public async Task SkipTeam_AnotherDoctorId_Test()
        {
            var (doctorId, teamId) = (Guid.NewGuid(), Guid.NewGuid());
            var doctors = new List<Doctor> { Fixtures.GetDoctorComposer(Guid.NewGuid()).Create() };
            var teamsMock = new List<InterdisciplinaryTeam>
            {
                Fixtures.GetInterdisciplinaryTeamComposer(teamId, isDeleted: false)
                    .With(pc => pc.Doctors, doctors).Create()
            };
            await _context.AddRangeAsync(teamsMock);
            await _context.SaveChangesAsync();

            var ids = await _queryHandler.GetInterdisciplinaryTeamIdsByDoctorId(doctorId);

            Assert.Empty(ids);
        }

        [Fact]
        public async Task FindTwoTeams_WithHealthCenterIdFilter_Successfully_Test()
        {
            var (doctorId, team1Id, team2Id) = (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var doctors = new List<Doctor> { Fixtures.GetDoctorComposer(doctorId).Create() };
            var teamsMock = new List<InterdisciplinaryTeam>()
            {
                Fixtures.GetInterdisciplinaryTeamComposer(team1Id, isDeleted: false)
                    .With(pc => pc.Doctors, doctors).Create(),
                Fixtures.GetInterdisciplinaryTeamComposer(team2Id, isDeleted: false)
                    .With(pc => pc.Doctors, doctors).Create(),
            };
            await _context.AddRangeAsync(teamsMock);
            await _context.SaveChangesAsync();

            var ids = (await _queryHandler.GetInterdisciplinaryTeamIdsByDoctorId(doctorId)).ToArray();

            Assert.Equal(2, ids.Length);
            Assert.Contains(team1Id, ids);
            Assert.Contains(team2Id, ids);
        }

        #endregion

        #region GetHealthCenterIdsByDoctorId

        [Fact]
        public async Task EmptyHealthCenterRepository_Test()
        {
            var doctorId = Guid.NewGuid();

            var ids = await _queryHandler.GetHealthCenterIdsByDoctorId(doctorId);

            Assert.Empty(ids);
        }

        [Fact]
        public async Task FindHealthCenter_Successfully_Test()
        {
            var (doctorId, healthCenterId) = (Guid.NewGuid(), Guid.NewGuid());
            var doctors = new List<Doctor>
            {
                Fixtures.GetDoctorComposer(doctorId).Create()
            };
            var healthCentersMock = new List<HealthCenter>
            {
                Fixtures.GetHealthCenterComposer(healthCenterId)
                    .With(pc => pc.Doctors, doctors).Create()
            };
            await _context.AddRangeAsync(healthCentersMock);
            await _context.SaveChangesAsync();

            var ids = (await _queryHandler.GetHealthCenterIdsByDoctorId(doctorId)).ToList();

            Assert.Single(ids);
            Assert.Equal(healthCenterId, ids.FirstOrDefault());
        }

        [Fact]
        public async Task SkipDeletedHealthCenter_Test()
        {
            var (doctorId, healthCenterId) = (Guid.NewGuid(), Guid.NewGuid());

            var doctors = new List<Doctor> { Fixtures.GetDoctorComposer(doctorId).Create() };
            var healthCentersMock = new List<HealthCenter>
                {
                    Fixtures.GetHealthCenterComposer(healthCenterId, isDeleted: true)
                        .With(pc => pc.Doctors, doctors).Create()
                };
            await _context.AddRangeAsync(healthCentersMock);
            await _context.SaveChangesAsync();

            var ids = (await _queryHandler.GetHealthCenterIdsByDoctorId(doctorId)).ToList();

            Assert.Empty(ids);
        }

        [Fact]
        public async Task SkipHealthCenter_AnotherDoctorId_Test()
        {
            var (doctorId, healthCenterId) = (Guid.NewGuid(), Guid.NewGuid());
            var doctors = new List<Doctor> { Fixtures.GetDoctorComposer(Guid.NewGuid()).Create() };
            var healthCentersMock = new List<HealthCenter>
            {
                Fixtures.GetHealthCenterComposer(healthCenterId, isDeleted: false)
                    .With(pc => pc.Doctors, doctors).Create()
            };
            await _context.AddRangeAsync(healthCentersMock);
            await _context.SaveChangesAsync();

            var ids = (await _queryHandler.GetHealthCenterIdsByDoctorId(doctorId)).ToList();

            Assert.Empty(ids);
        }

        #endregion

        #region GetPatientsByIds tests

        [Fact]
        public async Task EmptyPatientsRepository_WithoutFilter_Test()
        {
            var filterRequest = new PatientListFilterRequestDto()
            {
                DoctorId = Guid.NewGuid(),
                HealthCenterStructureFilter = HealthCenterStructureFilterType.InterdisciplinaryTeam
            };
            var query = new GetPatientsQuery(filterRequest, _defaultPagingRequest, SortingType.Asc);
            var interdisciplinaryTeams = new List<InterdisciplinaryTeam>()
            {
                Fixtures.GetInterdisciplinaryTeamComposer(Guid.NewGuid()).Create()
            };
            var doctors = new List<Doctor>()
            {
                Fixtures.GetDoctorComposer(Guid.NewGuid()).With(_ => _.InterdisciplinaryTeams, interdisciplinaryTeams).Create()
            };
            await _context.AddRangeAsync(doctors);
            await _context.AddRangeAsync(interdisciplinaryTeams);
            await _context.SaveChangesAsync();

            var patients = await _queryHandler.HandleAsync(query);

            Assert.Empty(patients.Items);
        }

        [Fact]
        public async Task FindPatient_ByInterdisciplinaryTeam_WithFilters_Successfully_Test()
        {
            var (teamId, doctorId) = (Guid.NewGuid(), Guid.NewGuid());
            var filterRequest = new PatientListFilterRequestDto
            {
                Name = "Abc",
                DoctorId = doctorId,
                HealthCenterStructureFilter = HealthCenterStructureFilterType.InterdisciplinaryTeam
            };
            var query = new GetPatientsQuery(filterRequest, _defaultPagingRequest, SortingType.Asc);
            var patientsMock = new List<Patient>()
            {
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abcd", "Defg", It.IsAny<string>(), "9856INV", interdisciplinaryTeamId: teamId).Create()
            };
            var doctors = new List<Doctor>()
            {
                Fixtures.GetDoctorComposer(doctorId).Create()
            };
            var interdisciplinaryTeams = new List<InterdisciplinaryTeam>()
            {
                Fixtures.GetInterdisciplinaryTeamComposer(teamId).With(_ => _.Doctors, doctors ).Create()
            };

            await _context.AddRangeAsync(interdisciplinaryTeams);
            await _context.AddRangeAsync(doctors);
            await _context.AddRangeAsync(patientsMock);
            await _context.SaveChangesAsync();

            var patients = await _queryHandler.HandleAsync(query);

            Assert.Single(patients.Items);
        }

        [Fact]
        public async Task FindPatient_ByHealthCenter_WithFilters_Successfully_Test()
        {
            var (healthCenter, doctorId) = (Guid.NewGuid(), Guid.NewGuid());
            var filterRequest = new PatientListFilterRequestDto
            {
                Name = "Abc",
                DoctorId = doctorId,
                HealthCenterStructureFilter = HealthCenterStructureFilterType.HealthCenter
            };
            var query = new GetPatientsQuery(filterRequest, _defaultPagingRequest, SortingType.Asc);
            var patientsMock = new List<Patient>()
            {
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abcd", "Defg", It.IsAny<string>(), "2534INV", null, healthCenter).Create()
            };
            var doctors = new List<Doctor>()
            {
                Fixtures.GetDoctorComposer(doctorId).Create()
            };
            var interdisciplinaryTeams = new List<HealthCenter>()
            {
                Fixtures.GetHealthCenterComposer(healthCenter).With(_ => _.Doctors, doctors ).Create()
            };

            await _context.AddRangeAsync(interdisciplinaryTeams);
            await _context.AddRangeAsync(doctors);
            await _context.AddRangeAsync(patientsMock);
            await _context.SaveChangesAsync();

            var patients = await _queryHandler.HandleAsync(query);

            Assert.Single(patients.Items);
        }

        [Fact]
        public async Task FindPatientUnsuccessfully_BecauseOfEmptyTeamIds_Test()
        {
            var doctorId = Guid.NewGuid();
            var patientListFilterRequest = new PatientListFilterRequestDto
            {
                Name = "Abc",
                DoctorId = doctorId,
                HealthCenterStructureFilter = HealthCenterStructureFilterType.InterdisciplinaryTeam
            };
            var query = new GetPatientsQuery(patientListFilterRequest, _defaultPagingRequest, SortingType.Asc);
            var patientsMock = new List<Patient>()
            {
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abcd", "Defg", It.IsAny<string>(), "123456789", null, Guid.NewGuid()).Create()
            };
            var doctors = new List<Doctor>()
            {
                Fixtures.GetDoctorComposer(doctorId).Create()
            };

            await _context.AddRangeAsync(doctors);
            await _context.AddRangeAsync(patientsMock);
            await _context.SaveChangesAsync();

            var patients = await _queryHandler.HandleAsync(query);

            Assert.Empty(patients.Items);
        }

        [Fact]
        public async Task FindPatientSuccessfully_ByName_Test()
        {
            var (teamId, doctorId) = (Guid.NewGuid(), Guid.NewGuid());
            var patientListFilterRequest = new PatientListFilterRequestDto
            {
                Name = "Abc",
                DoctorId = doctorId,
                HealthCenterStructureFilter = HealthCenterStructureFilterType.InterdisciplinaryTeam
            };
            var query = new GetPatientsQuery(patientListFilterRequest, _defaultPagingRequest, SortingType.Asc);
            var patientsMock = new List<Patient>()
            {
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abcd", "Defg", It.IsAny<string>(), "123456789", teamId).Create()
            };
            var doctors = new List<Doctor>()
            {
                Fixtures.GetDoctorComposer(doctorId).Create()
            };
            var interdisciplinaryTeams = new List<InterdisciplinaryTeam>()
            {
                Fixtures.GetInterdisciplinaryTeamComposer(teamId).With(_ => _.Doctors, doctors).Create()
            };

            await _context.AddRangeAsync(interdisciplinaryTeams);
            await _context.AddRangeAsync(doctors);
            await _context.AddRangeAsync(patientsMock);
            await _context.SaveChangesAsync();

            var patients = await _queryHandler.HandleAsync(query);

            Assert.Single(patients.Items);
        }

        [Fact]
        public async Task FindPatientUnsuccessfully_BecauseOfIncorrectTeamIdFilter_Test()
        {
            var (teamId, doctorId) = (Guid.NewGuid(), Guid.NewGuid());
            var patientListFilterRequest = new PatientListFilterRequestDto
            {
                Name = "Abc",
                DoctorId = doctorId,
                HealthCenterStructureFilter = HealthCenterStructureFilterType.InterdisciplinaryTeam
            };
            var query = new GetPatientsQuery(patientListFilterRequest, _defaultPagingRequest, SortingType.Asc);
            var patientsMock = new List<Patient>()
            {
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abcd", "Defg", It.IsAny<string>(), "123456789", teamId).Create()
            };
            var doctors = new List<Doctor>()
            {
                Fixtures.GetDoctorComposer(doctorId).Create()
            };
            var interdisciplinaryTeams = new List<InterdisciplinaryTeam>()
            {
                Fixtures.GetInterdisciplinaryTeamComposer(teamId).Create()
            };

            await _context.AddRangeAsync(interdisciplinaryTeams);
            await _context.AddRangeAsync(doctors);
            await _context.AddRangeAsync(patientsMock);
            await _context.SaveChangesAsync();

            var patients = await _queryHandler.HandleAsync(query);

            Assert.Empty(patients.Items);
        }


        [Fact]
        public async Task FindPatientUnsuccessfully_BecauseOfDeletedPatient_Test()
        {
            var (teamId, doctorId) = (Guid.NewGuid(), Guid.NewGuid());
            var patientListFilterRequest = new PatientListFilterRequestDto
            {
                Name = "Abc",
                DoctorId = doctorId,
                HealthCenterStructureFilter = HealthCenterStructureFilterType.InterdisciplinaryTeam
            };
            var query = new GetPatientsQuery(patientListFilterRequest, _defaultPagingRequest, SortingType.Asc);
            var patientsMock = new List<Patient>()
            {
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abcd", "Defg", It.IsAny<string>(), null, teamId, null, null, true).Create()
            };
            var doctors = new List<Doctor>()
            {
                Fixtures.GetDoctorComposer(doctorId).Create()
            };
            var interdisciplinaryTeams = new List<InterdisciplinaryTeam>()
            {
                Fixtures.GetInterdisciplinaryTeamComposer(teamId).With(_ => _.Doctors, doctors).Create()
            };

            await _context.AddRangeAsync(interdisciplinaryTeams);
            await _context.AddRangeAsync(doctors);
            await _context.AddRangeAsync(patientsMock);
            await _context.SaveChangesAsync();

            var patients = await _queryHandler.HandleAsync(query);

            Assert.Empty(patients.Items);
        }

        [Fact]
        public async Task FindPatientSuccessfully_Test()
        {
            var (teamId, doctorId) = (Guid.NewGuid(), Guid.NewGuid());
            var filterRequest = new PatientListFilterRequestDto()
            {
                DoctorId = doctorId,
                HealthCenterStructureFilter = HealthCenterStructureFilterType.InterdisciplinaryTeam
            };
            var query = new GetPatientsQuery(filterRequest, _defaultPagingRequest, SortingType.Asc);
            var patientsMock = new List<Patient>()
            {
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abcd", "Defg", It.IsAny<string>(), "123456789", null, teamId).Create()
            };
            var doctors = new List<Doctor>()
            {
                Fixtures.GetDoctorComposer(doctorId).Create()
            };
            var interdisciplinaryTeams = new List<InterdisciplinaryTeam>()
            {
                Fixtures.GetInterdisciplinaryTeamComposer(teamId).With(_ => _.Doctors, doctors).Create()
            };

            await _context.AddRangeAsync(interdisciplinaryTeams);
            await _context.AddRangeAsync(doctors);
            await _context.AddRangeAsync(patientsMock);
            await _context.SaveChangesAsync();

            var patients = await _queryHandler.HandleAsync(query);

            Assert.Single(patients.Items);
        }

        [Fact]
        public async Task FindPatientSuccessfully_WithPagingAndAscSorting_Test()
        {
            var (teamId, doctorId) = (Guid.NewGuid(), Guid.NewGuid());
            var filterRequest = new PatientListFilterRequestDto()
            {
                DoctorId = doctorId,
                HealthCenterStructureFilter = HealthCenterStructureFilterType.InterdisciplinaryTeam
            };
            var pagingRequest = new PagingRequestDto(2, 3);
            var query = new GetPatientsQuery(filterRequest, pagingRequest, SortingType.Asc);
            var patientsMock = new List<Patient>()
            {
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Dbcd", "Defg", It.IsAny<string>(), "123456789", null, teamId).Create(),
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Acd", "Sds", It.IsAny<string>(), "123435546", null, teamId).Create(),
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Cfds", "Dascv", It.IsAny<string>(), "123123563", null, teamId).Create(),
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Brfgdf", "Udsfdf", It.IsAny<string>(), "423425631", null, teamId).Create(),
            };
            var doctors = new List<Doctor>()
            {
                Fixtures.GetDoctorComposer(doctorId).Create()
            };
            var interdisciplinaryTeams = new List<InterdisciplinaryTeam>()
            {
                Fixtures.GetInterdisciplinaryTeamComposer(teamId).With(_ => _.Doctors, doctors).Create()
            };

            await _context.AddRangeAsync(interdisciplinaryTeams);
            await _context.AddRangeAsync(doctors);
            await _context.AddRangeAsync(patientsMock);
            await _context.SaveChangesAsync();

            var patients = (await _queryHandler.HandleAsync(query)).Items;

            Assert.Single(patients);
            Assert.Contains(patients, patient => patient.FirstName == "Dbcd");
        }

        [Fact]
        public async Task FindPatientSuccessfully_WithPagingAndDescSorting_Test()
        {
            var (teamId, doctorId) = (Guid.NewGuid(), Guid.NewGuid());
            var filterRequest = new PatientListFilterRequestDto()
            {
                DoctorId = doctorId,
                HealthCenterStructureFilter = HealthCenterStructureFilterType.InterdisciplinaryTeam
            };
            var pagingRequest = new PagingRequestDto(2, 3);
            var query = new GetPatientsQuery(filterRequest, pagingRequest, SortingType.Desc);
            var patientsMock = new List<Patient>()
            {
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Dbcd", "Defg", It.IsAny<string>(), "123456789", null, teamId).Create(),
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Acd", "Sds", It.IsAny<string>(), "123435546", null, teamId).Create(),
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Cfds", "Dascv", It.IsAny<string>(), "123123563", null, teamId).Create(),
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Brfgdf", "Udsfdf", It.IsAny<string>(), "423425631", null, teamId).Create(),
            };
            var doctors = new List<Doctor>()
            {
                Fixtures.GetDoctorComposer(doctorId).Create()
            };
            var interdisciplinaryTeams = new List<InterdisciplinaryTeam>()
            {
                Fixtures.GetInterdisciplinaryTeamComposer(teamId).With(_ => _.Doctors, doctors).Create()
            };

            await _context.AddRangeAsync(interdisciplinaryTeams);
            await _context.AddRangeAsync(doctors);
            await _context.AddRangeAsync(patientsMock);
            await _context.SaveChangesAsync();

            var patients = (await _queryHandler.HandleAsync(query)).Items;

            Assert.Single(patients);
            Assert.Contains(patients, patient => patient.FirstName == "Acd");
        }

        #endregion

        #region HadnleAsync Tests

        [Fact]
        public async Task FindPatientsSuccessfully_ByInterdisciplinaryTeam_Test()
        {
            // ARRANGE
            var (team1Id, team2Id, team3Id, doctorId, healthCenterId) = (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
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
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abce", "Sds", "", "12343a5546", team3Id, isDeleted: false).Create(), // teamId doesn't match
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abcd", "Defg", "", "12456789", team1Id, isDeleted: false).Create(), // correct (search by Name)
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abch", "Tdfd", "", "1234da25631", team2Id, isDeleted: false).Create(), // team is deleted
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abcf", "Dascv", "", "1231dd23563", team1Id, isDeleted: true).Create(), // patient is deleted
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Udsfdf", "Abcg", "", "123425631", team1Id, isDeleted: false).Create(), // correct, but skipped because of sorting and paging
            };
            await _context.AddRangeAsync(healthCenter);
            await _context.AddRangeAsync(teamsMock);
            await _context.AddRangeAsync(doctors);
            await _context.AddRangeAsync(patientsMock);
            await _context.SaveChangesAsync();

            var patientListFilterRequest = new PatientListFilterRequestDto
            {
                Name = "Abc",
                DoctorId = doctorId,
                HealthCenterStructureFilter = HealthCenterStructureFilterType.InterdisciplinaryTeam
            };
            var pagingRequest = new PagingRequestDto(1, 2);
            var query = new GetPatientsQuery(patientListFilterRequest, pagingRequest, SortingType.Desc);

            // ACT
            var patients = (await _queryHandler.HandleAsync(query)).Items;

            // ASSERT
            Assert.Single(patients);
            Assert.Contains(patients, patient => patient.FirstName == "Abcd"); // Udsfdf, Bcg, Abcd - First names without paging
        }

        [Fact]
        public async Task FindPatientsSuccessfully_ByHealthCenter_Test()
        {
            // ARRANGE
            var (healthCenter1Id, healthCenter2Id, healthCenter3Id, doctorId) = (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var doctors = new List<Doctor> { Fixtures.GetDoctorComposer(doctorId).Create() };
            var healthCenters = new List<HealthCenter>()
            {
                Fixtures.GetHealthCenterComposer(healthCenter1Id, isDeleted: false).With(pc => pc.Doctors, doctors).Create(),
                Fixtures.GetHealthCenterComposer(healthCenter2Id, isDeleted: true).With(pc => pc.Doctors, doctors).Create(),
                Fixtures.GetHealthCenterComposer(healthCenter3Id, isDeleted: true).Create(),
            };

            var patientsMock = new List<Patient>()
            {
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abce", "Sds", "", "12343a5546", null, healthCenter3Id).Create(), // teamId doesn't match
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abcd", "Defg", "", "123456789", null, healthCenter1Id).Create(), // correct (search by Name), but skipped because of sorting and paging
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abch", "Tdfd", "", "1234da25631", null, healthCenter2Id).Create(), // team is deleted
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Abcf", "Dascv", "", "1231dd23563", null, healthCenter1Id, null, true).Create(), // patient is deleted
                Fixtures.GetPatientComposer(Guid.NewGuid(), "Udsfdf", "Abcg", "", "123425631", null, healthCenter1Id).Create(), // correct (search by Name)
            };
            await _context.AddRangeAsync(healthCenters);
            await _context.AddRangeAsync(doctors);
            await _context.AddRangeAsync(patientsMock);
            await _context.SaveChangesAsync();

            var patientListFilterRequest = new PatientListFilterRequestDto
            {
                Name = "Abc",
                DoctorId = doctorId,
                HealthCenterStructureFilter = HealthCenterStructureFilterType.HealthCenter
            };
            var pagingRequest = new PagingRequestDto(1, 2);
            var query = new GetPatientsQuery(patientListFilterRequest, pagingRequest, SortingType.Asc);

            // ACT
            var patients = (await _queryHandler.HandleAsync(query)).Items; // Abcd, Bcg, Udsfdf - First names without paging

            // ASSERT
            Assert.Single(patients);
            Assert.Contains(patients, patient => patient.FirstName == "Udsfdf");
        }

        #endregion

        private static Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());

            return mockOptions;
        }
    }
}
