using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Commands;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Repositories;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure.Dto;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.Tests.CommandHandlers
{
    public class CreateBloodPressureMeasurementCommandHandlerTests : IDisposable
    {
        private readonly HealthMeasurementDomainServiceTestDbContext _context;
        private readonly CreateBloodPressureMeasurementCommandHandler _commandHandler;

        public CreateBloodPressureMeasurementCommandHandlerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<HealthMeasurementDomainServiceTestDbContext>()
                .UseInMemoryDatabase($"CreateBloodPressureMeasurementCommandHandlerTests-{Guid.NewGuid()}");

            _context = new HealthMeasurementDomainServiceTestDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());
            var writeRepository = new BloodPressureMeasurementWriteRepository(_context);
            var mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();
            _commandHandler = new CreateBloodPressureMeasurementCommandHandler(mapper, writeRepository);
        }

        [Fact]
        public async Task CreateBloodPressureMeasurement_ShouldReturnCreatedEntityId()
        {
            var command = new CreateMeasurementCommand<BloodPressureMeasurementDto>(Guid.NewGuid(), DateTime.Now,
                new BloodPressureMeasurementDto { Systolic = 120, Diastolic = 70, PulseRate = 60 });

            var createdId = await _commandHandler.HandleAsync(command);

            var newMeasurement = await _context.BloodPressureMeasurements.FindAsync(createdId);
            Assert.NotNull(newMeasurement);
            Assert.Equal(command.PatientId, newMeasurement.PatientId);
            Assert.Equal(command.Measure.Systolic, newMeasurement.Systolic);
            Assert.Equal(command.Measure.Diastolic, newMeasurement.Diastolic);
            Assert.Equal(command.Measure.PulseRate, newMeasurement.PulseRate);
            Assert.Equal(command.ClientDate, newMeasurement.ClientDate);
        }

        private Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());

            return mockOptions;
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
        }
    }
}
