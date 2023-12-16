using Moq;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;
using Telemedicine.Common.Contracts.GlobalContracts.Utilities;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common.Exceptions;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.User;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.QueryHandlers
{
    public class GetDoctorByInnerIdQueryHandlerTests
    {
        private readonly HealthCenterStructureDomainServiceDbContext _context;
        private readonly GetDoctorByInnerIdQueryHandler _queryHandler;

        public GetDoctorByInnerIdQueryHandlerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<HealthCenterStructureDomainServiceDbContext>()
                .UseInMemoryDatabase($"GetDoctorIdByInnerIdQueryHandlerTests-{Guid.NewGuid()}");

            _context = new HealthCenterStructureDomainServiceDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());

            var repository = new DoctorReadRepository(_context);

            var mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();

            _queryHandler = new GetDoctorByInnerIdQueryHandler(mapper, repository);
        }

        [Fact]
        public async Task HandleAsync_EmptyRepository_ShouldThrowEntityNotFoundException()
        {
            var innerId = "1234";
            var query = new GetDoctorByInnerIdQuery(innerId);

            var exception = await Record.ExceptionAsync(() => _queryHandler.HandleAsync(query));

            Assert.NotNull(exception);
            Assert.IsType<EntityNotFoundByInnerIdException>(exception);

            var customException = exception as EntityNotFoundByInnerIdException;
            Assert.Equal(innerId, customException!.InnerId);
            Assert.Equal(BusinessException.ErrorType.EntityNotFound.ToErrorCodeString(), customException.Code);
        }

        [Fact]
        public async Task HandleAsync_NoDoctor_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var innerId = "1234";
            var query = new GetDoctorByInnerIdQuery(innerId);

            var doctor = Fixtures.GetDoctorComposer(innerId: "43_21").Create();
            await _context.AddRangeAsync(doctor);
            await _context.SaveChangesAsync();

            // Act
            var exception = await Record.ExceptionAsync(() => _queryHandler.HandleAsync(query));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<EntityNotFoundByInnerIdException>(exception);

            var customException = exception as EntityNotFoundByInnerIdException;
            Assert.Equal(innerId, customException!.InnerId);
            Assert.Equal(BusinessException.ErrorType.EntityNotFound.ToErrorCodeString(), customException.Code);
        }

        [Fact]
        public async Task HandleAsync_DeletedDoctor_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            const string? innerId = "1234";
            var id = Guid.NewGuid();
            Doctor doctor = Fixtures.GetDoctorComposer(innerId: innerId, id: id, isDeleted: true).Create();
            await _context.AddAsync(doctor);
            await _context.SaveChangesAsync();

            var query = new GetDoctorByInnerIdQuery(innerId);

            // Act
            var exception = await Record.ExceptionAsync(() => _queryHandler.HandleAsync(query));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<EntityNotFoundByInnerIdException>(exception);

            var customException = exception as EntityNotFoundByInnerIdException;
            Assert.Equal(innerId, customException!.InnerId);
            Assert.Equal(BusinessException.ErrorType.EntityNotFound.ToErrorCodeString(), customException.Code);
        }

        [Fact]
        public async Task HandleAsync_DoctorExists_ShouldReturnCorrectDoctor()
        {
            // Arrange
            var innerId = "1234";
            var id = Guid.NewGuid();

            var doctor = Fixtures.GetDoctorComposer(innerId: innerId, id: id).Create();
            await _context.AddAsync(doctor);
            await _context.SaveChangesAsync();

            var query = new GetDoctorByInnerIdQuery(innerId);

            // Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(doctor.FirstName, result.FirstName);
            Assert.Equal(doctor.LastName, result.LastName);
            Assert.False(result.IsAdmin);
            Assert.Empty(result.HealthCenters);
        }

        [Fact]
        public async Task HandleAsync_DoctorIsAdminExists_ShouldReturnCorrectDoctor()
        {
            // Arrange
            var innerId = "1234";
            var id = Guid.NewGuid();

            var doctor = Fixtures.GetDoctorComposer(innerId: innerId, id: id, isAdmin: true).Create();
            var healthCenter1 = Fixtures.GetHealthCenterComposer().Create();
            var healthCenter2 = Fixtures.GetHealthCenterComposer().Create();
            // it will not be in the result
            var healthCenter3 = Fixtures.GetHealthCenterComposer().Create();

            doctor.SetHealthCenters(new[] { healthCenter1, healthCenter2 });

            await _context.AddAsync(healthCenter1);
            await _context.AddAsync(healthCenter2);
            await _context.AddAsync(healthCenter3);
            await _context.AddAsync(doctor);
            await _context.SaveChangesAsync();

            var query = new GetDoctorByInnerIdQuery(innerId);

            // Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(doctor.FirstName, result.FirstName);
            Assert.Equal(doctor.LastName, result.LastName);
            Assert.True(result.IsAdmin);
            Assert.Equal(2, result.HealthCenters.Count);
            Assert.Empty(result.HealthCenters.Select(_ => _.Id).Except(new[] { healthCenter1.Id, healthCenter2.Id }));
        }

        private static Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());

            return mockOptions;
        }
    }
}
