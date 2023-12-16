using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;
using Telemedicine.Common.Contracts.GlobalContracts.Utilities;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.User;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.QueryHandlers
{
    public class GetPatientByIdQueryHandlerTests
    {
        private readonly HealthCenterStructureDomainServiceDbContext _context;
        private readonly GetPatientByIdQueryHandler _queryHandler;

        public GetPatientByIdQueryHandlerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<HealthCenterStructureDomainServiceDbContext>()
                .UseInMemoryDatabase($"GetPatientByIdQueryHandlerTests-{Guid.NewGuid()}");

            _context = new HealthCenterStructureDomainServiceDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());

            var patientRepository = new PatientReadRepository(_context);

            IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();

            _queryHandler = new GetPatientByIdQueryHandler(mapper, patientRepository);
        }

        [Fact]
        public async Task HandleAsync_EmptyRepository_ShouldThrowEntityNotFoundException()
        {
            var patientId = Guid.NewGuid();
            var query = new GetPatientByIdQuery(patientId);

            var exception = await Record.ExceptionAsync(() => _queryHandler.HandleAsync(query));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<EntityNotFoundByIdException>(exception);

            var customException = exception as EntityNotFoundByIdException;
            Assert.Equal(patientId, customException!.Id);
            Assert.Equal(typeof(Patient).FullName, customException.Type);
            Assert.Equal(BusinessException.ErrorType.EntityNotFound.ToErrorCodeString(), customException.Code);
        }

        [Fact]
        public async Task HandleAsync_NoPatient_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var query = new GetPatientByIdQuery(patientId);

            var newPatient = Fixtures.GetPatientComposer(Guid.NewGuid(), isDeleted: false).Create();
            await _context.AddRangeAsync(newPatient);
            await _context.SaveChangesAsync();

            // Act
            var exception = await Record.ExceptionAsync(() => _queryHandler.HandleAsync(query));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<EntityNotFoundByIdException>(exception);

            var customException = exception as EntityNotFoundByIdException;
            Assert.Equal(patientId, customException!.Id);
            Assert.Equal(typeof(Patient).FullName, customException.Type);
            Assert.Equal(BusinessException.ErrorType.EntityNotFound.ToErrorCodeString(), customException.Code);
        }

        [Fact]
        public async Task HandleAsync_PatientExists_ShouldReturnCorrectPatient()
        {
            // Arrange
            var patientId = Guid.NewGuid();

            var healthCenter = Fixtures.GetHealthCenterComposer(Guid.NewGuid(), "Health Center for test", state: "California").Create();
            var careProvider = Fixtures.GetDoctorComposer(Guid.NewGuid(), "Emma", "Watson").Create();
            var newPatient = Fixtures.GetPatientComposer(patientId, isDeleted: false)
                .With(p => p.HealthCenter, healthCenter)
                .With(p => p.PrimaryCareProvider, careProvider)
                .Create();
            await _context.AddAsync(newPatient);
            await _context.SaveChangesAsync();

            var query = new GetPatientByIdQuery(patientId);

            // Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(patientId, result.Id);
            Assert.Equal(newPatient.FirstName, result.FirstName);
            Assert.Equal(newPatient.LastName, result.LastName);
            Assert.Equal(newPatient.PhoneNumber, result.PhoneNumber);
            Assert.Equal(newPatient.BirthDate, result.BirthDate);

            Assert.NotNull(result.HealthCenter);
            Assert.Equal(healthCenter.Id, result.HealthCenter.Id);
            Assert.Equal(healthCenter.Name, result.HealthCenter.Name);
            Assert.Equal(healthCenter.UsaState, result.HealthCenter.UsaState);

            Assert.NotNull(result.PrimaryCareProvider);
            Assert.Equal(careProvider.Id, result.PrimaryCareProvider!.Id);
            Assert.Equal(careProvider.FirstName, result.PrimaryCareProvider.FirstName);
            Assert.Equal(careProvider.LastName, result.PrimaryCareProvider.LastName);
        }

        [Fact]
        public async Task HandleAsync_PatientExistsWithAbsentPrimaryCareProvider_ShouldReturnCorrectPatient()
        {
            // Arrange
            var patientId = Guid.NewGuid();

            var healthCenter = Fixtures.GetHealthCenterComposer(Guid.NewGuid(), "Health Center for test", state: "California").Create();
            var newPatient = Fixtures.GetPatientComposer(patientId, isDeleted: false)
                .With(p => p.HealthCenter, healthCenter)
                .Create();
            await _context.AddAsync(newPatient);
            await _context.SaveChangesAsync();

            var query = new GetPatientByIdQuery(patientId);

            // Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(patientId, result.Id);
            Assert.Equal(newPatient.FirstName, result.FirstName);
            Assert.Equal(newPatient.LastName, result.LastName);
            Assert.Equal(newPatient.PhoneNumber, result.PhoneNumber);
            Assert.Equal(newPatient.BirthDate, result.BirthDate);

            Assert.NotNull(result.HealthCenter);
            Assert.Equal(healthCenter.Id, result.HealthCenter.Id);
            Assert.Equal(healthCenter.Name, result.HealthCenter.Name);
            Assert.Equal(healthCenter.UsaState, result.HealthCenter.UsaState);

            Assert.Null(result.PrimaryCareProvider);
        }

        private static Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());

            return mockOptions;
        }
    }
}
