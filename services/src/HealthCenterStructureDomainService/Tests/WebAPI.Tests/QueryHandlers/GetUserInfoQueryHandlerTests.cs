using System;
using System.Threading.Tasks;
using AutoFixture;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Enums;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.QueryHandlers
{
    public class GetUserInfoQueryHandlerTests
    {
        private readonly GetUserInfoQueryHandler _queryHandler;
        private readonly HealthCenterStructureDomainServiceDbContext _context;

        public GetUserInfoQueryHandlerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<HealthCenterStructureDomainServiceDbContext>()
                .UseInMemoryDatabase($"GetUserInfoQueryHandlerTests-{Guid.NewGuid()}");

            _context = new HealthCenterStructureDomainServiceDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());

            var userRepository = new UserReadRepository(_context);

            _queryHandler = new GetUserInfoQueryHandler(userRepository);
        }

        [Fact]
        public async Task GetUserInfoTest_Doctor_CommonSuccessfulTest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var doctor = Fixtures.GetDoctorComposer(userId).Create();
            await _context.AddRangeAsync(doctor);
            await _context.SaveChangesAsync();

            // Act
            var query = new GetUserInfoQuery(userId);
            var userInfo = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(userInfo);
            Assert.Equal(userId, userInfo.Id);
            Assert.Equal(UserType.Doctor, userInfo.Type);
            Assert.IsType<Doctor>(userInfo);

            var doctorInfo = userInfo as Doctor;
            Assert.NotNull(doctorInfo);
            Assert.Equal((int)API.Common.Common.UserType.Doctor, (int)userInfo.Type);
        }

        [Fact]
        public async Task GetUserInfoTest_Patient_CommonSuccessfulTest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var patient = Fixtures.GetPatientComposer(userId).Create();

            await _context.AddRangeAsync(patient);
            await _context.SaveChangesAsync();

            // Act
            var query = new GetUserInfoQuery(userId);
            var userInfo = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(userInfo);
            Assert.Equal(userId, userInfo.Id);
            Assert.Equal(UserType.Patient, userInfo.Type);
            Assert.IsType<Patient>(userInfo);

            var patientInfo = userInfo as Patient;
            Assert.NotNull(patientInfo);
            Assert.Equal(patientInfo?.BirthDate, patient.BirthDate);
            Assert.Equal((int)API.Common.Common.UserType.Patient, (int)userInfo.Type);
        }

        [Fact]
        public async Task GetUserInfoTest_NotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var patient = Fixtures.GetPatientComposer(Guid.NewGuid()).Create();

            await _context.AddRangeAsync(patient);
            await _context.SaveChangesAsync();

            // Act
            var query = new GetUserInfoQuery(userId);
            var exception = await Record.ExceptionAsync(() => _queryHandler.HandleAsync(query));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<EntityNotFoundByIdException>(exception);
        }

        [Fact]
        public async Task GetUserInfoTest_EmptyRepository()
        {
            // Act
            var query = new GetUserInfoQuery(Guid.NewGuid());

            var exception = await Record.ExceptionAsync(() => _queryHandler.HandleAsync(query));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<EntityNotFoundByIdException>(exception);
        }

        private static Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());

            return mockOptions;
        }
    }
}
