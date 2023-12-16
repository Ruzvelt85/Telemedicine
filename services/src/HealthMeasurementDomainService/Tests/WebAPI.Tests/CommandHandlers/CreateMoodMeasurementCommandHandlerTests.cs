using System;
using System.Threading.Tasks;
using AutoMapper;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Commands;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Repositories;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Exceptions;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.Tests.CommandHandlers
{
    public class CreateMoodMeasurementCommandHandlerTests : IDisposable
    {
        private readonly CreateMoodMeasurementCommandHandler _commandHandler;
        private readonly HealthMeasurementDomainServiceTestDbContext _context;

        public CreateMoodMeasurementCommandHandlerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<HealthMeasurementDomainServiceTestDbContext>()
                .UseInMemoryDatabase($"CreateMoodMeasurementCommandHandlerTests-{Guid.NewGuid()}");

            _context = new HealthMeasurementDomainServiceTestDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());

            var moodMeasurementWriteRepository = new MoodMeasurementWriteRepository(_context);
            var moodMeasurementReadRepository = new MoodMeasurementReadRepository(_context);
            var options = Options.Create(new TimeZoneSettings());
            var timeZoneProvider = new SettingsTimeZoneProvider(options);
            var mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();
            _commandHandler = new CreateMoodMeasurementCommandHandler(mapper, moodMeasurementReadRepository, moodMeasurementWriteRepository, timeZoneProvider);
        }

        [Fact]
        public async Task CreateMoodMeasurement_ShouldReturnCreatedEntity()
        {
            // Arrange
            var command = new CreateMeasurementCommand<MoodMeasurementDto>(
                Guid.NewGuid(),
                DateTime.UtcNow,
                new MoodMeasurementDto { Measure = MoodMeasureType.Happy });

            // Act
            var createdId = await _commandHandler.HandleAsync(command);

            // Assert
            var newMeasurement = await _context.MoodMeasurements.FindAsync(createdId);
            Assert.NotNull(newMeasurement);
            Assert.Equal(command.PatientId, newMeasurement.PatientId);
            Assert.Equal((int)command.Measure.Measure, (int)newMeasurement.Measure);
            Assert.Equal(command.ClientDate, newMeasurement.ClientDate);
        }

        [Theory]
        [InlineData("2021-12-01 05:59:59", "2021-12-01 06:00:00")]
        [InlineData("2021-12-01 06:00:00", "2021-12-02 06:00:00")]
        [InlineData("2021-12-01 06:00:01", "2021-12-02 06:00:00")]
        public async Task CreateMoodMeasurement_TwiceInDifferentDays_ByCSTTimeZone_ShouldReturnCreatedEntity(DateTime firstDateTime, DateTime secondDateTime)
        {
            // Arrange
            var firstCommand = new CreateMeasurementCommand<MoodMeasurementDto>(
                Guid.NewGuid(),
                firstDateTime,
                new MoodMeasurementDto() { Measure = MoodMeasureType.Happy });
            var secondCommand = new CreateMeasurementCommand<MoodMeasurementDto>(
                firstCommand.PatientId,
                secondDateTime,
                new MoodMeasurementDto() { Measure = MoodMeasureType.Happy });

            // Act
            var firstCreatedId = await _commandHandler.HandleAsync(firstCommand);
            var secondCreatedId = await _commandHandler.HandleAsync(secondCommand);

            // Assert
            var newFirstMeasurement = await _context.MoodMeasurements.FindAsync(firstCreatedId);
            var newSecondMeasurement = await _context.MoodMeasurements.FindAsync(secondCreatedId);
            Assert.NotNull(newFirstMeasurement);
            Assert.Equal(firstCommand.PatientId, newFirstMeasurement.PatientId);
            Assert.Equal((int)firstCommand.Measure.Measure, (int)newFirstMeasurement.Measure);
            Assert.Equal(firstCommand.ClientDate, newFirstMeasurement.ClientDate);
            Assert.NotNull(newSecondMeasurement);
            Assert.Equal(secondCommand.PatientId, newSecondMeasurement.PatientId);
            Assert.Equal((int)secondCommand.Measure.Measure, (int)newSecondMeasurement.Measure);
            Assert.Equal(secondCommand.ClientDate, newSecondMeasurement.ClientDate);
        }

        [Theory]
        [InlineData("2021-12-01 06:00:00", "2021-12-01 06:00:00")]
        [InlineData("2021-12-01 06:00:00", "2021-12-01 06:00:01")]
        [InlineData("2021-12-01 06:00:00", "2021-12-02 05:59:59")]
        public async Task CreateMoodMeasurement_TwiceInOneDay_ByCSTTimeZone_ShouldThrowMoodAlreadyCreatedTodayException(DateTime firstDateTime, DateTime secondDateTime)
        {
            // Arrange
            var firstCommand = new CreateMeasurementCommand<MoodMeasurementDto>(
                Guid.NewGuid(),
                firstDateTime,
                new MoodMeasurementDto() { Measure = MoodMeasureType.Happy });
            var secondCommand = new CreateMeasurementCommand<MoodMeasurementDto>(
                firstCommand.PatientId,
                secondDateTime,
                new MoodMeasurementDto() { Measure = MoodMeasureType.Happy });

            // Act
            var createdId = await _commandHandler.HandleAsync(firstCommand);

            // Assert
            var newMeasurement = await _context.MoodMeasurements.FindAsync(createdId);
            Assert.NotNull(newMeasurement);
            await _context.SaveChangesAsync();
            await Assert.ThrowsAsync<MoodAlreadyCreatedTodayException>(() => _commandHandler.HandleAsync(secondCommand));
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
