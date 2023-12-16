using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Common.Infrastructure.EntityBase;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Exceptions;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;
using Telemedicine.Services.AppointmentDomainService.Core.Events;
using Telemedicine.Services.AppointmentDomainService.Core.Services;
using Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.CommandHandlers.TestSetup;
using Telemedicine.Services.AppointmentDomainService.WebAPI;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Commands;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Repositories;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.CommandHandlers
{
    public class RescheduleAppointmentCommandHandlerTests : AppointmentCommandHandlerTests
    {
        public RescheduleAppointmentCommandHandlerTests() : base($"{nameof(RescheduleAppointmentCommandHandlerTests)}-{Guid.NewGuid()}")
        {
        }

        [Fact]
        public async Task RescheduleAppointment_EmptyRepository_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var command = new Fixture().Build<RescheduleAppointmentCommand>().Create();
            var domainEventPublisherSpy = new DomainEventPublisherSpy<DomainEvent>();
            IOverlappedAppointmentsService overlappedAppointmentService = new Mock<IOverlappedAppointmentsService>(MockBehavior.Loose).Object;

            await using (var context = await GetDbContext())
            {
                IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();
                var commandHandler = new RescheduleAppointmentCommandHandler(mapper, new AppointmentReadRepository(context), new AppointmentWriteRepository(context), overlappedAppointmentService);

                //Act
                var ex = await Record.ExceptionAsync(() => commandHandler.HandleAsync(command));
                var unitOfWork = new UnitOfWork<AppointmentDomainServiceDbContext>(context, domainEventPublisherSpy);
                await unitOfWork.SaveAsync();

                //Assert
                Assert.NotNull(ex);
                Assert.IsType<EntityNotFoundByIdException>(ex);
            }

            // Assert
            await using (var context = await GetDbContext())
            {
                context.Appointments.Should().HaveCount(0);
            }

            domainEventPublisherSpy.ShouldPublishNumberOfEvents(0);
        }

        [Fact]
        public async Task RescheduleAppointment_WhenOverlapped_ShouldThrowOverlappedException()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid();
            var appointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.StartDate, DateTime.UtcNow.AddDays(1))
                .With(_ => _.Duration, TimeSpan.FromHours(1))
                .With(_ => _.IsDeleted, false)
                .Create();
            await AddAsync(appointment);

            var command = new Fixture().Build<RescheduleAppointmentCommand>()
                .With(_ => _.Id, appointmentId)
                .Create();
            var domainEventPublisherSpy = new DomainEventPublisherSpy<DomainEvent>();
            var mockService = new Mock<IOverlappedAppointmentsService>();
            mockService
                .Setup(_ => _.GetOverlappedAppointments(It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<Guid[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Appointment[] { new Fixture().Build<Appointment>().Create() });
            IOverlappedAppointmentsService overlappedAppointmentService = mockService.Object;
            await using (var context = await GetDbContext())
            {
                IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();
                var commandHandler = new RescheduleAppointmentCommandHandler(mapper, new AppointmentReadRepository(context), new AppointmentWriteRepository(context), overlappedAppointmentService);

                //Act
                var ex = await Record.ExceptionAsync(() => commandHandler.HandleAsync(command));
                var unitOfWork = new UnitOfWork<AppointmentDomainServiceDbContext>(context, domainEventPublisherSpy);
                await unitOfWork.SaveAsync();

                //Assert
                Assert.NotNull(ex);
                Assert.IsType<AppointmentOverlappedException>(ex);
            }

            // Assert
            await using (var context = await GetDbContext())
            {
                context.Appointments.Should().HaveCount(1);
            }

            domainEventPublisherSpy.ShouldPublishNumberOfEvents(0);
        }

        [Fact]
        public async Task RescheduleAppointment_WhenRescheduleCancelledApp_ShouldThrowInvalidAppointmentStateException()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid();
            var appointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.StartDate, DateTime.UtcNow.AddDays(1))
                .With(_ => _.Duration, TimeSpan.FromHours(1))
                .With(_ => _.Status, AppointmentStatus.Cancelled)
                .With(_ => _.IsDeleted, false)
                .Create();
            await AddAsync(appointment);

            var command = new Fixture().Build<RescheduleAppointmentCommand>()
                .With(_ => _.Id, appointmentId)
                .Create();
            var domainEventPublisherSpy = new DomainEventPublisherSpy<DomainEvent>();
            IOverlappedAppointmentsService overlappedAppointmentService = new Mock<IOverlappedAppointmentsService>(MockBehavior.Loose).Object;
            await using (var context = await GetDbContext())
            {
                IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();
                var commandHandler = new RescheduleAppointmentCommandHandler(mapper, new AppointmentReadRepository(context), new AppointmentWriteRepository(context), overlappedAppointmentService);

                //Act
                var ex = await Record.ExceptionAsync(() => commandHandler.HandleAsync(command));
                var unitOfWork = new UnitOfWork<AppointmentDomainServiceDbContext>(context, domainEventPublisherSpy);
                await unitOfWork.SaveAsync();

                //Assert
                Assert.NotNull(ex);
                Assert.IsType<InvalidAppointmentStateException>(ex);
                var stateEx = ex as InvalidAppointmentStateException;
                Assert.Equal(appointmentId, stateEx!.AppointmentId);
                Assert.Equal(API.Common.Common.AppointmentState.Cancelled, stateEx!.AppointmentState);
            }

            // Assert
            await using (var context = await GetDbContext())
            {
                context.Appointments.Should().HaveCount(1);
            }

            domainEventPublisherSpy.ShouldPublishNumberOfEvents(0);
        }

        [Fact]
        public async Task RescheduleAppointment_NoOverlapped_ShouldReturnNewRescheduledAppointmentId()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid(), newAppointmentId;
            var appointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.StartDate, DateTime.UtcNow.AddDays(1))
                .With(_ => _.Duration, TimeSpan.FromHours(1))
                .With(_ => _.IsDeleted, false)
                .Create();

            await AddAsync(appointment);
            var command = new Fixture().Build<RescheduleAppointmentCommand>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.StartDate, DateTime.UtcNow.AddDays(1))
                .Create();

            var domainEventPublisherSpy = new DomainEventPublisherSpy<DomainEvent>();
            IOverlappedAppointmentsService overlappedAppointmentService = new Mock<IOverlappedAppointmentsService>(MockBehavior.Loose).Object;

            // Act
            await using (var context = await GetDbContext())
            {
                IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();
                var commandHandler = new RescheduleAppointmentCommandHandler(mapper, new AppointmentReadRepository(context), new AppointmentWriteRepository(context), overlappedAppointmentService);

                newAppointmentId = await commandHandler.HandleAsync(command);

                var unitOfWork = new UnitOfWork<AppointmentDomainServiceDbContext>(context, domainEventPublisherSpy);
                await unitOfWork.SaveAsync();
            }

            // Assert
            await using (var context = await GetDbContext())
            {
                var oldAppointment = await context.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == appointmentId);
                oldAppointment.Should().NotBeNull();
                oldAppointment!.GetState().Should().Be(AppointmentState.Cancelled);

                var newAppointment = await context.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == newAppointmentId);
                newAppointment.Should().NotBeNull();
                newAppointment!.GetState().Should().Be(AppointmentState.Opened);
            }

            domainEventPublisherSpy.ShouldPublishNumberOfEvents(2)
                .And.ShouldContain(domainEvent => domainEvent is AppointmentCreatedDomainEvent)
                .And.ShouldContain(domainEvent => domainEvent is AppointmentCancelledDomainEvent);
        }
    }
}
