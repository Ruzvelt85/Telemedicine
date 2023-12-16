using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using VidyoService;
using Xunit;
using AutoMapper;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Common.Infrastructure.VidyoClient;
using Telemedicine.Common.Infrastructure.VidyoClient.Exceptions;
using Telemedicine.Services.VideoConfIntegrService.Core.Repositories;
using Telemedicine.Services.VideoConfIntegrService.DAL;
using Telemedicine.Services.VideoConfIntegrService.WebAPI;
using Telemedicine.Services.VideoConfIntegrService.WebAPI.Commands;
using Telemedicine.Services.VideoConfIntegrService.WebAPI.Repositories;
using Telemedicine.Services.VideoConfIntegrService.WebAPI.Services;
using CreateConferenceException = Telemedicine.Common.Infrastructure.VidyoClient.Exceptions.CreateConferenceException;

namespace Telemedicine.Services.VideoConfIntegrService.Tests.WebAPI.Tests.Commands
{
    public class CreateConferenceCommandHandlerTests : IDisposable
    {
        private readonly VideoConferenceIntegrationServiceDbContext _context;
        private readonly IConferenceWriteRepository _writeRepository;
        private readonly IMapper _mapper;

        public CreateConferenceCommandHandlerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<VideoConferenceIntegrationServiceDbContext>()
                .UseInMemoryDatabase($"CreateVideoConferenceCommandHandlerTests-{Guid.NewGuid()}");

            _context = new VideoConferenceIntegrationServiceDbContext(dbContextOptions.Options, GetMockOptionsForEfCore().Object, new NullLoggerFactory());
            _writeRepository = new ConferenceWriteRepository(_context);

            _mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();
        }

        [Fact]
        public async Task CreateConference_CommonSuccessfulTest_ShouldCreateConference()
        {
            // Arrange
            var vidyoClientMock = new Mock<IVidyoClient>();
            vidyoClientMock
                .Setup(client => client.CreateRoom(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new CreateRoomResponse()
                {
                    Entity = new Entity()
                    {
                        entityID = "123",
                        RoomMode = new RoomMode { roomURL = "https://test.com", hasPIN = false, roomPIN = string.Empty }
                    }
                });
            vidyoClientMock
                .Setup(client => client.SetPinCode(It.IsAny<string>(), It.IsAny<string>()));

            var conferenceBuilder = new ConferenceFactory(_mapper, GetMockOptionsForConference().Object, vidyoClientMock.Object);
            var commandHandler = new CreateConferenceCommandHandler(conferenceBuilder, _writeRepository);

            // Act
            var command = new CreateConferenceCommand(Guid.NewGuid(), "Title", DateTime.UtcNow.AddDays(1), TimeSpan.FromHours(1));

            var createdId = await commandHandler.HandleAsync(command);

            // Assert
            var newConference = await _context.Conferences.FindAsync(createdId);
            Assert.NotNull(newConference);
            Assert.Equal(command.AppointmentId, newConference.AppointmentId);
            Assert.Equal(command.AppointmentTitle, newConference.AppointmentTitle);
            Assert.Equal(command.AppointmentStartDate, newConference.AppointmentStartDate);
            Assert.Equal(command.AppointmentDuration, newConference.AppointmentDuration);
            Assert.NotEmpty(newConference.RoomUrl);
            Assert.True(newConference.RoomId > 0);
            Assert.NotNull(newConference.RoomPin);
        }

        [Fact]
        public async Task CreateConference_CreateRoomFailed_ShouldThrowException()
        {
            // Arrange
            var vidyoClientMock = new Mock<IVidyoClient>();
            vidyoClientMock
                .Setup(client => client.CreateRoom(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => throw new CreateConferenceException("Create Room Failed"));
            vidyoClientMock
                .Setup(client => client.SetPinCode(It.IsAny<string>(), It.IsAny<string>()));

            var conferenceBuilder = new ConferenceFactory(_mapper, GetMockOptionsForConference().Object, vidyoClientMock.Object);
            var commandHandler = new CreateConferenceCommandHandler(conferenceBuilder, _writeRepository);

            // Act
            var command = new CreateConferenceCommand(Guid.NewGuid(), "Title", DateTime.UtcNow.AddDays(1), TimeSpan.FromHours(1));
            var exception = await Record.ExceptionAsync(() => commandHandler.HandleAsync(command));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<API.Common.VideoConferenceCommandService.Exceptions.CreateConferenceException>(exception);
        }

        [Fact]
        public async Task CreateConference_CreateRoomFailedDueToInvalidSettings_ShouldThrowException()
        {
            // Arrange
            var vidyoClientMock = new Mock<IVidyoClient>();
            vidyoClientMock
                .Setup(client => client.CreateRoom(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => throw new CreateConferenceWithInvalidParametersException("Create Room Failed"));
            vidyoClientMock
                .Setup(client => client.SetPinCode(It.IsAny<string>(), It.IsAny<string>()));

            var conferenceBuilder = new ConferenceFactory(_mapper, GetMockOptionsForConference().Object, vidyoClientMock.Object);
            var commandHandler = new CreateConferenceCommandHandler(conferenceBuilder, _writeRepository);

            // Act
            var command = new CreateConferenceCommand(Guid.NewGuid(), "Title", DateTime.UtcNow.AddDays(1), TimeSpan.FromHours(1));
            var exception = await Record.ExceptionAsync(() => commandHandler.HandleAsync(command));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<API.Common.VideoConferenceCommandService.Exceptions.CreateConferenceException>(exception);
        }

        [Fact]
        public async Task CreateConference_SetPinCodeFailed_ShouldCreateConference()
        {
            // Arrange
            var vidyoClientMock = new Mock<IVidyoClient>();
            vidyoClientMock
                .Setup(client => client.CreateRoom(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new CreateRoomResponse()
                {
                    Entity = new Entity()
                    {
                        entityID = "123",
                        RoomMode = new RoomMode { roomURL = "https://test.com", hasPIN = false, roomPIN = string.Empty }
                    }
                });
            vidyoClientMock
                .Setup(client => client.SetPinCode(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => throw new SetPinCodeException("Set Pin failed"));

            var conferenceBuilder = new ConferenceFactory(_mapper, GetMockOptionsForConference().Object, vidyoClientMock.Object);
            var commandHandler = new CreateConferenceCommandHandler(conferenceBuilder, _writeRepository);

            // Act
            var command = new CreateConferenceCommand(Guid.NewGuid(), "Title", DateTime.UtcNow.AddDays(1), TimeSpan.FromHours(1));

            var createdId = await commandHandler.HandleAsync(command);

            // Assert
            var newConference = await _context.Conferences.FindAsync(createdId);
            Assert.NotNull(newConference);
            Assert.Equal(command.AppointmentId, newConference.AppointmentId);
            Assert.Equal(command.AppointmentTitle, newConference.AppointmentTitle);
            Assert.Equal(command.AppointmentStartDate, newConference.AppointmentStartDate);
            Assert.Equal(command.AppointmentDuration, newConference.AppointmentDuration);
            Assert.NotEmpty(newConference.RoomUrl);
            Assert.True(newConference.RoomId > 0);
            Assert.Null(newConference.RoomPin);
        }

        private Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptionsForEfCore()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());
            return mockOptions;
        }

        private static Mock<IOptionsSnapshot<ConferenceSettings>> GetMockOptionsForConference()
        {
            var mockOptions = new Mock<IOptionsSnapshot<ConferenceSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new ConferenceSettings
            {
                ExtensionPrefix = "123456",
                IsSetPinCode = true,
                PinCodeFormat = 8
            });
            return mockOptions;
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
        }
    }
}
