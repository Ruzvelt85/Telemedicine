using AutoFixture;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.HealthCenter;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.User;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Enums;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.Doctor;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.CommandHandlers
{
    public class CreateOrUpdateDoctorCommandTests
    {
        private readonly HealthCenterStructureDomainServiceDbContext _context;
        private readonly CreateOrUpdateDoctorCommandHandler _commandHandler;

        public CreateOrUpdateDoctorCommandTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<HealthCenterStructureDomainServiceDbContext>()
                .UseInMemoryDatabase($"CreateOrUpdateDoctorCommandTests-{Guid.NewGuid()}");

            _context = new HealthCenterStructureDomainServiceDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());
            _context.Database.EnsureCreated();

            IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();
            _commandHandler = new CreateOrUpdateDoctorCommandHandler(mapper, new DoctorReadRepository(_context),
                new DoctorWriteRepository(_context), new HealthCenterReadRepository(_context));
        }

        [Fact]
        public async Task CreateDoctor_EmptyRepository_ShouldReturnCreatedDoctorId()
        {
            // Arrange
            var command = new Fixture().Build<CreateOrUpdateDoctorCommand>()
                .With(_ => _.IsActive, true)
                .Create();

            // Act
            var newId = await _commandHandler.HandleAsync(command);
            await _context.SaveChangesAsync();

            var doctor = await _context.Doctors.AsNoTracking().FirstOrDefaultAsync();
            Assert.NotNull(doctor);
            Assert.Equal(doctor.Id, newId);
            Assert.Equal(UserType.Doctor, doctor.Type);
        }

        [Fact]
        public async Task UpdateDoctor_ShouldReturnCreatedDoctorId()
        {
            // Arrange
            var doctorToUpdate = Fixtures.GetDoctorComposer(innerId: "doctor inner id",
                firstName: "first name", lastName: "last name").Create();
            await _context.Doctors.AddAsync(doctorToUpdate);

            var testHealthCenter = Fixtures.GetHealthCenterComposer(innerId: "test health center inner id",
                name: "health center name", state: "NY").Create();
            await _context.HealthCenters.AddAsync(testHealthCenter);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var command = new CreateOrUpdateDoctorCommand(doctorToUpdate.InnerId, "First name Changed", "Last Name Changed",
                new[] { testHealthCenter.InnerId }, true);

            // Act
            var newId = await _commandHandler.HandleAsync(command);
            await _context.SaveChangesAsync();

            // Assert
            var updatedDoctor = await _context.Doctors.FirstAsync(_ => _.InnerId == doctorToUpdate.InnerId);
            Assert.NotNull(updatedDoctor);
            Assert.Equal(updatedDoctor.Id, newId);
            Assert.Equal(command.FirstName, updatedDoctor.FirstName);
            Assert.Equal(command.LastName, updatedDoctor.LastName);
            Assert.Equal(command.InnerId, updatedDoctor.InnerId);
            Assert.Equal(UserType.Doctor, updatedDoctor.Type);
            Assert.Equal(1, updatedDoctor.HealthCenters.Count);
            Assert.NotNull(updatedDoctor.HealthCenters);

            foreach (HealthCenter doctorHealthCenter in updatedDoctor.HealthCenters)
            {
                Assert.Equal(testHealthCenter.InnerId, doctorHealthCenter.InnerId);
                Assert.Equal(testHealthCenter.Name, doctorHealthCenter.Name);
                Assert.Equal(testHealthCenter.UsaState, doctorHealthCenter.UsaState);
            }
        }


        [Theory]
        [InlineData(new string[0], new[] { "1", "2", "3" })]
        [InlineData(new[] { "1", "2" }, new[] { "3", "4" })]
        [InlineData(new[] { "1", "2" }, new string[0])]
        public async Task UpdateDoctor_WithNoMatchingHealthCenter_ShouldReturnCreatedDoctorId(string[] existingPcInnerIds, string[] commandPcInnerIds)
        {
            await UpdateDoctor_WithHealthCenter_ShouldReturnCreatedDoctorId(existingPcInnerIds, commandPcInnerIds, Array.Empty<string>());
        }

        [Theory]
        [InlineData(new[] { "1" }, new[] { "1" }, new[] { "1" })]
        [InlineData(new[] { "1", "2" }, new[] { "1", "2" }, new[] { "1", "2" })]
        public async Task UpdateDoctor_WithSameHealthCenter_ShouldReturnCreatedDoctorId(string[] existingPcInnerIds, string[] commandPcInnerIds, string[] expectedPcInnerIds)
        {
            await UpdateDoctor_WithHealthCenter_ShouldReturnCreatedDoctorId(existingPcInnerIds, commandPcInnerIds, expectedPcInnerIds);
        }


        [Theory]
        [InlineData(new[] { "1" }, new string[0], new string[0])]
        [InlineData(new[] { "1", "2" }, new[] { "1" }, new[] { "1" })]
        [InlineData(new[] { "1", "2", "3" }, new[] { "1", "2" }, new[] { "1", "2" })]
        [InlineData(new[] { "1", "2", "3" }, new[] { "1", "4" }, new[] { "1" })]
        public async Task UpdateDoctor_WithReducingNumberOfHealthCenters_ShouldReturnCreatedDoctorId(string[] existingPcInnerIds, string[] commandPcInnerIds, string[] expectedPcInnerIds)
        {
            await UpdateDoctor_WithHealthCenter_ShouldReturnCreatedDoctorId(existingPcInnerIds, commandPcInnerIds, expectedPcInnerIds);
        }

        [Theory]
        [InlineData(new string[0], new[] { "1", "2" }, new string[0])]
        [InlineData(new[] { "1" }, new[] { "1", "2", "3" }, new[] { "1" })]
        [InlineData(new[] { "1", "2" }, new[] { "1", "2", "3" }, new[] { "1", "2" })]
        [InlineData(new[] { "1", "2" }, new[] { "1", "2", "4" }, new[] { "1", "2" })]
        [InlineData(new[] { "1", "2", "3" }, new[] { "1", "2", "4" }, new[] { "1", "2" })]
        public async Task UpdateDoctor_WithMoreHealthCentersInCommandThanExisting_ShouldReturnCreatedDoctorId(string[] existingPcInnerIds, string[] commandPcInnerIds, string[] expectedPcInnerIds)
        {
            await UpdateDoctor_WithHealthCenter_ShouldReturnCreatedDoctorId(existingPcInnerIds, commandPcInnerIds, expectedPcInnerIds);
        }

        private async Task UpdateDoctor_WithHealthCenter_ShouldReturnCreatedDoctorId(string[] existingPcInnerIds, string[] commandPcInnerIds, string[] expectedPcInnerIds)
        {
            // Arrange
            var doctorToUpdate = Fixtures.GetDoctorComposer(innerId: "doctor inner id").Create();
            await _context.Doctors.AddAsync(doctorToUpdate);

            foreach (string innerId in existingPcInnerIds)
            {
                var testHealthCenter = Fixtures.GetHealthCenterComposer(innerId: innerId, name: $"health center name {innerId}", state: "NY").Create();
                testHealthCenter.Doctors.Add(doctorToUpdate);
                await _context.HealthCenters.AddAsync(testHealthCenter);
            }

            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var command = new CreateOrUpdateDoctorCommand(doctorToUpdate.InnerId, "First name Changed", "Last Name Changed", commandPcInnerIds, true);

            // Act
            var newId = await _commandHandler.HandleAsync(command);
            await _context.SaveChangesAsync();

            // Assert
            var updatedDoctor = await _context.Doctors.FirstAsync(_ => _.InnerId == doctorToUpdate.InnerId);
            Assert.NotNull(updatedDoctor);
            Assert.Equal(updatedDoctor.Id, newId);
            Assert.Equal(command.FirstName, updatedDoctor.FirstName);
            Assert.Equal(command.LastName, updatedDoctor.LastName);
            Assert.Equal(command.InnerId, updatedDoctor.InnerId);
            Assert.Equal(UserType.Doctor, updatedDoctor.Type);
            Assert.NotNull(updatedDoctor.HealthCenters);
            var healthCenters = updatedDoctor.HealthCenters;
            Assert.Equal(expectedPcInnerIds.Length, healthCenters.Count);
            Assert.True(healthCenters.All(pc => expectedPcInnerIds.Contains(pc.InnerId)));
        }

        [Fact]
        public async Task UpdateDoctorWithHealthCenters_ShouldReturnCreatedDoctorIdAndRemoveOldHealthCenters()
        {
            // Arrange
            var initialHealthCenter = Fixtures.GetHealthCenterComposer(innerId: "initial health center inner id",
                name: "initial health center name", state: "NY").Create();
            await _context.HealthCenters.AddAsync(initialHealthCenter);

            var doctorToUpdate = Fixtures.GetDoctorComposer(innerId: "doctor inner id",
                firstName: "first name", lastName: "last name").Create();
            doctorToUpdate.SetHealthCenters(new[] { initialHealthCenter });

            var newHealthCenter = Fixtures.GetHealthCenterComposer(innerId: "new health center inner id",
                name: "new health center name", state: "NY").Create();
            await _context.HealthCenters.AddAsync(newHealthCenter);

            await _context.Doctors.AddAsync(doctorToUpdate);
            await _context.SaveChangesAsync();

            _context.ChangeTracker.Clear();

            var command = new CreateOrUpdateDoctorCommand(doctorToUpdate.InnerId, "First name Changed", "Last Name Changed",
                new[] { newHealthCenter.InnerId }, true);

            // Act
            var newId = await _commandHandler.HandleAsync(command);
            await _context.SaveChangesAsync();

            // Assert
            var updatedDoctor = await _context.Doctors.FirstOrDefaultAsync(_ => _.InnerId == doctorToUpdate.InnerId);
            Assert.NotNull(updatedDoctor);
            Assert.Equal(updatedDoctor.Id, newId);
            Assert.Equal(command.FirstName, updatedDoctor.FirstName);
            Assert.Equal(command.LastName, updatedDoctor.LastName);
            Assert.Equal(command.InnerId, updatedDoctor.InnerId);
            Assert.Equal(UserType.Doctor, updatedDoctor.Type);
            Assert.Equal(1, updatedDoctor.HealthCenters.Count);
            Assert.NotNull(updatedDoctor.HealthCenters);

            foreach (HealthCenter doctorHealthCenter in updatedDoctor.HealthCenters)
            {
                Assert.Equal(newHealthCenter.InnerId, doctorHealthCenter.InnerId);
                Assert.Equal(newHealthCenter.Name, doctorHealthCenter.Name);
                Assert.Equal(newHealthCenter.UsaState, doctorHealthCenter.UsaState);
            }
        }

        private static Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(_ => _.Value).Returns(new EfCoreDbContextSettings());

            return mockOptions;
        }
    }
}
