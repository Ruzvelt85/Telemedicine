using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Xunit;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Commands;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Repositories;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation.Dto;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.Tests.CommandHandlers
{
    public class CreateSaturationMeasurementCommandHandlerTests : IDisposable
    {
        private readonly HealthMeasurementDomainServiceTestDbContext _context;
        private readonly ICommandHandler<CreateMeasurementCommand<SaturationMeasurementDto>, Guid> _commandHandler;

        public CreateSaturationMeasurementCommandHandlerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<HealthMeasurementDomainServiceTestDbContext>()
                .UseInMemoryDatabase($"CreateSaturationMeasurementCommandHandlerTests-{Guid.NewGuid()}");

            _context = new HealthMeasurementDomainServiceTestDbContext(dbContextOptions.Options, GetEfCoreMockOptions().Object, new NullLoggerFactory());

            var saturationMeasurementWriteRepository = new SaturationMeasurementWriteRepository(_context);
            var mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();

            _commandHandler = new CreateSaturationMeasurementCommandHandler(mapper, saturationMeasurementWriteRepository);
        }

        [Fact]
        public async Task CreateSaturationMeasurement_WithEmptyRawSaturation_ShouldReturnCreatedEntityId()
        {
            // Arrange
            var command = new CreateMeasurementCommand<SaturationMeasurementDto>(Guid.NewGuid(), DateTime.Now,
                new SaturationMeasurementDto
                {
                    SpO2 = 1,
                    PulseRate = 2,
                    Pi = 3,
                    RawMeasurements = Array.Empty<RawSaturationMeasurementItemDto>()
                });

            // Act
            var createdId = await _commandHandler.HandleAsync(command);
            await _context.SaveChangesAsync();

            // Assert
            SaturationMeasurement newMeasurement = await _context.SaturationMeasurements.FindAsync(createdId);
            Assert.NotNull(newMeasurement);
            Assert.Equal(command.PatientId, newMeasurement.PatientId);
            Assert.Equal(command.Measure.SpO2, newMeasurement.SpO2);
            Assert.Equal(command.Measure.PulseRate, newMeasurement.PulseRate);
            Assert.Equal(command.Measure.Pi, newMeasurement.Pi);
            Assert.NotNull(newMeasurement.RawSaturationData);
            Assert.Empty(newMeasurement.RawSaturationData!.Items);
            Assert.Equal(command.ClientDate, newMeasurement.ClientDate);
        }

        [Fact]
        public async Task CreateSaturationMeasurement_WithNullRawSaturation_ShouldReturnCreatedEntityId()
        {
            // Arrange
            var command = new CreateMeasurementCommand<SaturationMeasurementDto>(Guid.NewGuid(), DateTime.Now,
                new SaturationMeasurementDto
                {
                    SpO2 = 1,
                    PulseRate = 2,
                    Pi = 3
                });

            // Act
            var createdId = await _commandHandler.HandleAsync(command);
            await _context.SaveChangesAsync();

            // Assert
            SaturationMeasurement newMeasurement = await _context.SaturationMeasurements.FindAsync(createdId);
            Assert.NotNull(newMeasurement);
            Assert.Equal(command.PatientId, newMeasurement.PatientId);
            Assert.Equal(command.Measure.SpO2, newMeasurement.SpO2);
            Assert.Equal(command.Measure.PulseRate, newMeasurement.PulseRate);
            Assert.Equal(command.Measure.Pi, newMeasurement.Pi);
            Assert.Null(newMeasurement.RawSaturationData);
            Assert.Equal(command.ClientDate, newMeasurement.ClientDate);
        }

        [Fact]
        public async Task CreateSaturationMeasurement_WithRawSaturationItems_ShouldReturnCreatedEntityId()
        {
            // Arrange
            var rawSaturationItems = new[]
            {
                new RawSaturationMeasurementItemDto
                {
                    Order = 1, SpO2 = 2, PulseRate = 3, Pi = 4.0m, ClientDate = DateTime.Now
                },
                new RawSaturationMeasurementItemDto
                {
                    Order = 2, SpO2 = 3, PulseRate = 4, Pi = 5.6m, ClientDate = DateTime.Now
                },
                new RawSaturationMeasurementItemDto
                {
                    Order = 3, SpO2 = 4, PulseRate = 5, Pi = 6.6m, ClientDate = DateTime.Now
                },
            };

            var command = new CreateMeasurementCommand<SaturationMeasurementDto>(Guid.NewGuid(), DateTime.Now,
                new SaturationMeasurementDto
                {
                    SpO2 = 1,
                    PulseRate = 2,
                    Pi = 3,
                    RawMeasurements = rawSaturationItems
                });

            // Act
            var createdId = await _commandHandler.HandleAsync(command);
            await _context.SaveChangesAsync();

            // Assert
            SaturationMeasurement newMeasurement = await _context.SaturationMeasurements
                .Include(_ => _.RawSaturationData)
                .FirstOrDefaultAsync(x => x.Id == createdId);

            Assert.NotNull(newMeasurement);
            Assert.Equal(command.PatientId, newMeasurement.PatientId);
            Assert.Equal(command.Measure.SpO2, newMeasurement.SpO2);
            Assert.Equal(command.Measure.PulseRate, newMeasurement.PulseRate);
            Assert.Equal(command.Measure.Pi, newMeasurement.Pi);
            Assert.Equal(rawSaturationItems.Length, newMeasurement.RawSaturationData!.Items.Count);
            CompareRawCollections(rawSaturationItems, newMeasurement.RawSaturationData.Items);
            Assert.Equal(command.ClientDate, newMeasurement.ClientDate);
        }

        private static void CompareRawCollections(IReadOnlyCollection<RawSaturationMeasurementItemDto>? expectedCollection,
            ICollection<RawSaturationItem>? actualCollection)
        {
            if (expectedCollection == null)
                return;

            var expectedCollectionArray = expectedCollection.OrderBy(_ => _.Order)
                .ToArray();
            var actualCollectionArray = actualCollection!.OrderBy(_ => _.Order)
                .ToArray();

            for (int i = 0; i < expectedCollection.Count; i++)
            {
                Assert.Equal(expectedCollectionArray[i].Order, actualCollectionArray[i].Order);
                Assert.Equal(expectedCollectionArray[i].PulseRate, actualCollectionArray[i].PulseRate);
                Assert.Equal(expectedCollectionArray[i].Pi, actualCollectionArray[i].Pi);
                Assert.Equal(expectedCollectionArray[i].SpO2, actualCollectionArray[i].SpO2);
                Assert.Equal(expectedCollectionArray[i].ClientDate, actualCollectionArray[i].ClientDate);
            }
        }

        private static Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetEfCoreMockOptions()
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
