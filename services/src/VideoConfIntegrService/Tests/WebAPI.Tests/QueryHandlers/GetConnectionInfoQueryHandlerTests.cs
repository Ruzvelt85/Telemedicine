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
using Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceQueryService.Exceptions;
using Telemedicine.Services.VideoConfIntegrService.Core.Entities;
using Telemedicine.Services.VideoConfIntegrService.DAL;
using Telemedicine.Services.VideoConfIntegrService.Tests.Tests.Core;
using Telemedicine.Services.VideoConfIntegrService.WebAPI;
using Telemedicine.Services.VideoConfIntegrService.WebAPI.Queries;
using Telemedicine.Services.VideoConfIntegrService.WebAPI.Repositories;

namespace Telemedicine.Services.VideoConfIntegrService.Tests.WebAPI.Tests.QueryHandlers
{
    public class GetConnectionInfoQueryHandlerTests
    {
        private readonly VideoConferenceIntegrationServiceDbContext _context;
        private readonly GetConnectionInfoQueryHandler _queryHandler;

        public GetConnectionInfoQueryHandlerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<VideoConferenceIntegrationServiceDbContext>()
                .UseInMemoryDatabase($"GetConnectionInfoQueryHandlerTests-{Guid.NewGuid()}");

            _context = new VideoConferenceIntegrationServiceDbContext(dbContextOptions.Options,
                GetMockOptions().Object,
                new NullLoggerFactory());

            var conferenceReadRepository = new ConferenceReadRepository(_context);

            IMapper mapper = new MapperConfiguration(
                cfg => cfg.AddMaps(typeof(Startup).Assembly))
                .CreateMapper();

            _queryHandler = new GetConnectionInfoQueryHandler(mapper, conferenceReadRepository);
        }

        [Fact]
        public async Task HandleAsync_EmptyRepository_ShouldThrowEntityNotFoundException()
        {
            var appointmentId = Guid.NewGuid();
            var query = new GetConnectionInfoQuery(appointmentId);

            var exception = await Record.ExceptionAsync(() => _queryHandler.HandleAsync(query));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<VideoConferenceNotFoundByAppointmentIdException>(exception);

            var customException = exception as VideoConferenceNotFoundByAppointmentIdException;
            Assert.Equal(appointmentId, customException!.AppointmentId);
            Assert.Equal(BusinessException.ErrorType.EntityNotFound.ToErrorCodeString(), customException.Code);
        }

        [Fact]
        public async Task HandleAsync_NoConference_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var query = new GetConnectionInfoQuery(appointmentId);

            var conference = new Fixture()
                .Build<Conference>()
                .With(e => e.AppointmentId, Guid.NewGuid())
                .Create();

            await _context.AddRangeAsync(conference);
            await _context.SaveChangesAsync();

            // Act
            var exception = await Record.ExceptionAsync(() => _queryHandler.HandleAsync(query));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<VideoConferenceNotFoundByAppointmentIdException>(exception);

            var customException = exception as VideoConferenceNotFoundByAppointmentIdException;
            Assert.Equal(appointmentId, customException!.AppointmentId);
            Assert.Equal(BusinessException.ErrorType.EntityNotFound.ToErrorCodeString(), customException.Code);
        }

        [Fact]
        public async Task HandleAsync_ConferenceExists_ShouldReturnCorrectConnectionInfo()
        {
            // Arrange
            var conference = Fixtures.CreateConferenceFixture(
                Guid.NewGuid(),
                "test",
                123,
                "https://test.com/hdo",
                "12345");

            await _context.AddAsync(conference);
            await _context.SaveChangesAsync();

            var query = new GetConnectionInfoQuery(conference.AppointmentId);

            // Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(conference.AppointmentId, result.AppointmentId);
            Assert.Equal(conference.RoomId, result.RoomId);
            Assert.Equal("https://test.com", result.Host);
            Assert.Equal("hdo", result.RoomKey);
            Assert.Equal(conference.RoomPin, result.RoomPin);
        }

        private static Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());

            return mockOptions;
        }
    }
}
