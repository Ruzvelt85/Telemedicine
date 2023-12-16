using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using AutoFixture;
using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.HealthCenter;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.HealthCenter;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.CommandHandlers
{
    public class CreateOrUpdateHealthCenterCommandHandlerTests
    {
        private readonly CreateOrUpdateHealthCenterCommandHandler _commandHandler;
        private readonly HealthCenterStructureDomainServiceDbContext _context;

        public CreateOrUpdateHealthCenterCommandHandlerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<HealthCenterStructureDomainServiceDbContext>()
                .UseInMemoryDatabase($"CreateOrUpdateHealthCenterCommandHandlerTests-{Guid.NewGuid()}");
            IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();
            _context = new HealthCenterStructureDomainServiceDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());
            _commandHandler = new CreateOrUpdateHealthCenterCommandHandler(mapper, new HealthCenterReadRepository(_context), new HealthCenterWriteRepository(_context));
        }

        [Fact]
        public async Task CreateHealthCenter_EmptyRepository()
        {
            // Act
            var command = new CreateOrUpdateHealthCenterCommand("1001_VA", "Roanoke", "VA", true);
            var response = await _commandHandler.HandleAsync(command);

            // Assert
            Assert.NotEqual(Guid.Empty, response);

            var savedHealthCenter = await _context.HealthCenters.FindAsync(response);
            Assert.NotNull(savedHealthCenter);
            Assert.Equal("Roanoke", savedHealthCenter.Name);
            Assert.Equal("1001_VA", savedHealthCenter.InnerId);
            Assert.Equal("VA", savedHealthCenter.UsaState);
            Assert.False(savedHealthCenter.IsDeleted);
        }

        [Fact]
        public async Task CreateHealthCenter_CommonSuccessfulTest()
        {
            // Arrange
            var healthCenter1 = Fixtures.GetHealthCenterComposer(innerId: "1010_PA", name: "Germantown").Create();
            var healthCenter2 = Fixtures.GetHealthCenterComposer(innerId: "1007_CO", name: "Loveland").Create();
            await _context.AddRangeAsync(healthCenter1, healthCenter2);
            await _context.SaveChangesAsync();
            Assert.Equal(2, await _context.HealthCenters.CountAsync());

            // Act
            var command = new CreateOrUpdateHealthCenterCommand("1001_VA", "Roanoke", "VA", true);
            var response = await _commandHandler.HandleAsync(command);
            await _context.SaveChangesAsync();

            // Assert
            Assert.NotEqual(Guid.Empty, response);
            Assert.Equal(3, await _context.HealthCenters.CountAsync());
            var savedHealthCenter = await _context.HealthCenters.FindAsync(response);
            Assert.NotNull(savedHealthCenter);
            Assert.Equal("Roanoke", savedHealthCenter.Name);
            Assert.Equal("1001_VA", savedHealthCenter.InnerId);
            Assert.Equal("VA", savedHealthCenter.UsaState);
            Assert.False(savedHealthCenter.IsDeleted);
        }

        [Fact]
        public async Task UpdateHealthCenter_CommonSuccessfulTest()
        {
            // Arrange
            var healthCenter1 = Fixtures.GetHealthCenterComposer(innerId: "1010_PA", name: "Germantown").Create();
            var healthCenter2 = Fixtures.GetHealthCenterComposer(innerId: "1007_CO", name: "Loveland", state: "CO").Create();
            await _context.HealthCenters.AddRangeAsync(healthCenter1, healthCenter2);
            await _context.SaveChangesAsync();
            Assert.Equal(2, _context.HealthCenters.Count());
            _context.Entry(healthCenter2).State = EntityState.Detached;

            // Act
            var command = new CreateOrUpdateHealthCenterCommand("1007_CO", "Roanoke", "VA", true);
            var response = await _commandHandler.HandleAsync(command);
            await _context.SaveChangesAsync();

            // Assert
            Assert.NotEqual(Guid.Empty, response);
            Assert.Equal(2, _context.HealthCenters.Count());

            var updatedHealthCenter = _context.HealthCenters.Where(x => x.Id == response).ToList().First();
            Assert.NotNull(updatedHealthCenter);
            Assert.Equal("Roanoke", updatedHealthCenter.Name);
            Assert.Equal("1007_CO", updatedHealthCenter.InnerId);
            Assert.Equal("VA", updatedHealthCenter.UsaState);
            Assert.False(updatedHealthCenter.IsDeleted);
        }

        [Fact]
        public async Task UpdateDeletedHealthCenter_CommonSuccessfulTest()
        {
            // Arrange
            var healthCenter1 = Fixtures.GetHealthCenterComposer(innerId: "1010_PA", name: "Germantown").Create();
            var healthCenter2 = Fixtures.GetHealthCenterComposer(innerId: "1007_CO", name: "Loveland", state: "CO", isDeleted: true).Create();
            await _context.HealthCenters.AddRangeAsync(healthCenter1, healthCenter2);
            await _context.SaveChangesAsync();
            Assert.Equal(2, _context.HealthCenters.IgnoreQueryFilters().Count());
            _context.Entry(healthCenter2).State = EntityState.Detached;

            // Act
            var command = new CreateOrUpdateHealthCenterCommand("1007_CO", "Loveland", "CO", true);
            var response = await _commandHandler.HandleAsync(command);
            await _context.SaveChangesAsync();

            // Assert
            Assert.NotEqual(Guid.Empty, response);
            Assert.Equal(2, _context.HealthCenters.IgnoreQueryFilters().Count());

            var updatedHealthCenter = _context.HealthCenters.IgnoreQueryFilters().Where(x => x.Id == response).ToList().First();
            Assert.NotNull(updatedHealthCenter);
            Assert.Equal("Loveland", updatedHealthCenter.Name);
            Assert.Equal("1007_CO", updatedHealthCenter.InnerId);
            Assert.Equal("CO", updatedHealthCenter.UsaState);
            Assert.False(updatedHealthCenter.IsDeleted);
        }


        private static Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());

            return mockOptions;
        }
    }
}
