using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using AutoFixture;
using System;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common.Exceptions;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.Patient;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.HealthCenter;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.User;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.CommandHandlers
{
    public class CreateOrUpdatePatientCommandHandlerTests
    {
        private readonly CreateOrUpdatePatientCommandHandler _commandHandler;
        private readonly HealthCenterStructureDomainServiceDbContext _context;

        public CreateOrUpdatePatientCommandHandlerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<HealthCenterStructureDomainServiceDbContext>()
                .UseInMemoryDatabase($"CreateOrUpdatePatientCommandHandlerTests-{Guid.NewGuid()}");
            IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();
            _context = new HealthCenterStructureDomainServiceDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());
            _commandHandler = new CreateOrUpdatePatientCommandHandler(mapper, new PatientReadRepository(_context), new PatientWriteRepository(_context), new DoctorReadRepository(_context), new HealthCenterReadRepository(_context));
        }

        [Fact]
        public async Task CreatePatient_EmptyRepository_WithEmptyPcp()
        {
            // Act
            var healthCenterToAdd = Fixtures.GetHealthCenterComposer(innerId: "1001_NM").Create();
            var healthCenter = await _context.AddAsync(healthCenterToAdd);
            await _context.SaveChangesAsync();

            var command = new CreateOrUpdatePatientCommand("22651INV", "Randle", "Bob", "2158733087", new DateTime(1928, 8, 5), "1001_NM", null, true);
            var response = await _commandHandler.HandleAsync(command);

            // Assert
            Assert.NotEqual(Guid.Empty, response);

            var savedPatient = await _context.Patients.FindAsync(response);
            Assert.NotNull(savedPatient);
            Assert.Equal("22651INV", savedPatient.InnerId);
            Assert.Equal("Randle", savedPatient.LastName);
            Assert.Equal("Bob", savedPatient.FirstName);
            Assert.Equal(new DateTime(1928, 8, 5), savedPatient.BirthDate);
            Assert.Equal("2158733087", savedPatient.PhoneNumber);
            Assert.Equal(healthCenter.Entity.Id, savedPatient.HealthCenterId);
            Assert.Null(savedPatient.PrimaryCareProviderId);
            Assert.False(savedPatient.IsDeleted);
        }

        [Fact]
        public async Task CreatePatient_CommonSuccessfulTest()
        {
            // Arrange
            var patient1 = Fixtures.GetPatientComposer(innerId: "16449INV").Create();
            var patient2 = Fixtures.GetPatientComposer(innerId: "13377INV").Create();
            await _context.AddRangeAsync(patient1, patient2);
            await _context.SaveChangesAsync();
            Assert.Equal(2, await _context.Patients.CountAsync());

            var healthCenterToAdd = Fixtures.GetHealthCenterComposer(innerId: "1001_NM").Create();
            var healthCenter = await _context.AddAsync(healthCenterToAdd);
            await _context.SaveChangesAsync();

            var doctorToAdd = Fixtures.GetDoctorComposer(innerId: "1995_CO").Create();
            var doctor = await _context.AddAsync(doctorToAdd);
            await _context.SaveChangesAsync();

            // Act
            var command = new CreateOrUpdatePatientCommand("22266INV", "Lowe", "Richard", "3033552515", new DateTime(1930, 9, 27), "1001_NM", "1995_CO", true);
            var response = await _commandHandler.HandleAsync(command);
            await _context.SaveChangesAsync();

            // Assert
            Assert.NotEqual(Guid.Empty, response);

            var savedPatient = await _context.Patients.FindAsync(response);
            Assert.NotNull(savedPatient);
            Assert.Equal("22266INV", savedPatient.InnerId);
            Assert.Equal("Lowe", savedPatient.LastName);
            Assert.Equal("Richard", savedPatient.FirstName);
            Assert.Equal(new DateTime(1930, 9, 27), savedPatient.BirthDate);
            Assert.Equal("3033552515", savedPatient.PhoneNumber);
            Assert.Equal(healthCenter.Entity.Id, savedPatient.HealthCenterId);
            Assert.NotNull(savedPatient.PrimaryCareProviderId);
            Assert.Equal(doctor.Entity.Id, savedPatient.PrimaryCareProviderId);
            Assert.False(savedPatient.IsDeleted);
        }

        [Fact]
        public async Task UpdatePatient_CommonSuccessfulTest()
        {
            // Arrange
            var patient1 = Fixtures.GetPatientComposer(innerId: "16449INV").Create();
            var patient2 = Fixtures.GetPatientComposer(innerId: "13377INV").Create();
            await _context.AddRangeAsync(patient1, patient2);
            await _context.SaveChangesAsync();
            Assert.Equal(2, await _context.Patients.CountAsync());
            _context.Entry(patient2).State = EntityState.Detached;

            var healthCenterToAdd = Fixtures.GetHealthCenterComposer(innerId: "1001_NM").Create();
            var healthCenter = await _context.AddAsync(healthCenterToAdd);
            await _context.SaveChangesAsync();

            var doctorToAdd = Fixtures.GetDoctorComposer(innerId: "2268_CO").Create();
            var doctor = await _context.AddAsync(doctorToAdd);
            await _context.SaveChangesAsync();

            // Act
            var command = new CreateOrUpdatePatientCommand("13377INV", "Lowe", "Richard", "3033552515", new DateTime(1930, 9, 27), "1001_NM", "2268_CO", true);
            var response = await _commandHandler.HandleAsync(command);
            await _context.SaveChangesAsync();

            // Assert
            Assert.NotEqual(Guid.Empty, response);

            var updatedPatient = await _context.Patients.FindAsync(response);
            Assert.NotNull(updatedPatient);
            Assert.Equal("13377INV", updatedPatient.InnerId);
            Assert.Equal("Lowe", updatedPatient.LastName);
            Assert.Equal("Richard", updatedPatient.FirstName);
            Assert.Equal(new DateTime(1930, 9, 27), updatedPatient.BirthDate);
            Assert.Equal("3033552515", updatedPatient.PhoneNumber);
            Assert.Equal(healthCenter.Entity.Id, updatedPatient.HealthCenterId);
            Assert.NotNull(updatedPatient.PrimaryCareProviderId);
            Assert.Equal(doctor.Entity.Id, updatedPatient.PrimaryCareProviderId);
            Assert.False(updatedPatient.IsDeleted);
        }

        [Fact]
        public async Task UpdateDeletedPatient_CommonSuccessfulTest()
        {
            // Arrange
            var patient1 = Fixtures.GetPatientComposer(innerId: "16449INV").Create();
            var patient2 = Fixtures.GetPatientComposer(innerId: "13377INV", isDeleted: true).Create();
            await _context.AddRangeAsync(patient1, patient2);
            await _context.SaveChangesAsync();
            Assert.Equal(2, await _context.Patients.IgnoreQueryFilters().CountAsync());
            _context.Entry(patient2).State = EntityState.Detached;

            var healthCenterToAdd = Fixtures.GetHealthCenterComposer(innerId: "1001_NM").Create();
            var healthCenter = await _context.AddAsync(healthCenterToAdd);
            await _context.SaveChangesAsync();

            var doctorToAdd = Fixtures.GetDoctorComposer(innerId: "1995_CO").Create();
            var doctor = await _context.AddAsync(doctorToAdd);
            await _context.SaveChangesAsync();

            // Act
            var command = new CreateOrUpdatePatientCommand("13377INV", "Lowe", "Richard", "3033552515", new DateTime(1930, 9, 27), "1001_NM", "1995_CO", true);
            var response = await _commandHandler.HandleAsync(command);
            await _context.SaveChangesAsync();

            // Assert
            Assert.NotEqual(Guid.Empty, response);

            var updatedPatient = await _context.Patients.FindAsync(response);
            Assert.NotNull(updatedPatient);
            Assert.Equal("13377INV", updatedPatient.InnerId);
            Assert.Equal("Lowe", updatedPatient.LastName);
            Assert.Equal("Richard", updatedPatient.FirstName);
            Assert.Equal(new DateTime(1930, 9, 27), updatedPatient.BirthDate);
            Assert.Equal("3033552515", updatedPatient.PhoneNumber);
            Assert.Equal(healthCenter.Entity.Id, updatedPatient.HealthCenterId);
            Assert.NotNull(updatedPatient.PrimaryCareProviderId);
            Assert.Equal(doctor.Entity.Id, updatedPatient.PrimaryCareProviderId);
            Assert.False(updatedPatient.IsDeleted);
        }

        [Fact]
        public async Task UpdatePatient_WithNotExistingHealthCenter_ShouldThrowAnException()
        {
            // Arrange
            var patient1 = Fixtures.GetPatientComposer(innerId: "16449INV").Create();
            var patient2 = Fixtures.GetPatientComposer(innerId: "13377INV").Create();
            await _context.AddRangeAsync(patient1, patient2);
            await _context.SaveChangesAsync();
            Assert.Equal(2, await _context.Patients.CountAsync());
            _context.Entry(patient2).State = EntityState.Detached;

            var healthCenterToAdd = Fixtures.GetHealthCenterComposer(innerId: "1001_NM").Create();
            await _context.AddAsync(healthCenterToAdd);
            await _context.SaveChangesAsync();

            var doctorToAdd = Fixtures.GetDoctorComposer(innerId: "2268_CO").Create();
            await _context.AddAsync(doctorToAdd);
            await _context.SaveChangesAsync();

            string notExistingInnerId = "test_inner_id";
            // Act
            var command = new CreateOrUpdatePatientCommand("13377INV", "Lowe", "Richard", "3033552515", new DateTime(1930, 9, 27), notExistingInnerId, "2268_CO", true);
            var ex = await Record.ExceptionAsync(() => _commandHandler.HandleAsync(command));

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<EntityNotFoundByInnerIdException>(ex);
        }

        private static Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());

            return mockOptions;
        }
    }
}
