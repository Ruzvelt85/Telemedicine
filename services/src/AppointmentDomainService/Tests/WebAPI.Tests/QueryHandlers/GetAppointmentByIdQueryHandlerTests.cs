using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;
using Telemedicine.Common.Contracts.GlobalContracts.Utilities;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.WebAPI;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Queries;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.QueryHandlers
{
    public class GetAppointmentByIdQueryHandlerTests
    {
        private readonly AppointmentDomainServiceDbContext _context;
        private readonly GetAppointmentByIdQueryHandler _queryHandler;

        public GetAppointmentByIdQueryHandlerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<AppointmentDomainServiceDbContext>()
                .UseInMemoryDatabase($"GetAppointmentByIdQueryHandlerTests-{Guid.NewGuid()}");

            _context = new AppointmentDomainServiceDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());

            var appointmentReadRepository = new AppointmentReadRepository(_context);

            IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();

            _queryHandler = new GetAppointmentByIdQueryHandler(mapper, appointmentReadRepository);
        }

        [Fact]
        public async Task HandleAsync_EmptyRepository_ShouldThrowEntityNotFoundException()
        {
            var appointmentId = Guid.NewGuid();
            var query = new GetAppointmentByIdQuery(appointmentId);

            var exception = await Record.ExceptionAsync(() => _queryHandler.HandleAsync(query));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<EntityNotFoundByIdException>(exception);

            var customException = exception as EntityNotFoundByIdException;
            Assert.Equal(appointmentId, customException!.Id);
            Assert.Equal(typeof(Appointment).FullName, customException.Type);
            Assert.Equal(BusinessException.ErrorType.EntityNotFound.ToErrorCodeString(), customException.Code);
        }

        [Fact]
        public async Task HandleAsync_NoAppointment_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var query = new GetAppointmentByIdQuery(appointmentId);

            var appointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, Guid.NewGuid)
                .With(_ => _.IsDeleted, false)
                .Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();

            // Act
            var exception = await Record.ExceptionAsync(() => _queryHandler.HandleAsync(query));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<EntityNotFoundByIdException>(exception);

            var customException = exception as EntityNotFoundByIdException;
            Assert.Equal(appointmentId, customException!.Id);
            Assert.Equal(typeof(Appointment).FullName, customException.Type);
            Assert.Equal(BusinessException.ErrorType.EntityNotFound.ToErrorCodeString(), customException.Code);
        }

        [Fact]
        public async Task HandleAsync_AppointmentExists_ShouldReturnCorrectAppointment()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var query = new GetAppointmentByIdQuery(appointmentId);

            var appointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.IsDeleted, false)
                .Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(appointmentId, result.Id);
            Assert.Equal(appointment.Title, result.Title);
            Assert.Equal(appointment.Description, result.Description);
            Assert.Equal(appointment.StartDate, result.StartDate);
            Assert.Equal(appointment.Duration, result.Duration);
            Assert.Equal(appointment.Type.ToString(), result.Type.ToString());
            Assert.Equal(appointment.GetState().ToString(), result.State.ToString());
            Assert.Equal(appointment.Rating, result.Rating);
            Assert.Equal(appointment.Attendees.Count(_ => !_.IsDeleted), result.Attendees.Count);
        }

        private static Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());

            return mockOptions;
        }
    }
}
