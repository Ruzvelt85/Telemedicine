using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Telemedicine.Common.Contracts.GlobalContracts.Utilities;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
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
using Telemedicine.Services.AppointmentDomainService.WebAPI.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;


namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.CommandHandlers
{
    public class CreateAppointmentCommandHandlerTests : AppointmentCommandHandlerTests
    {
        public CreateAppointmentCommandHandlerTests() : base($"{nameof(CreateAppointmentCommandHandlerTests)}-{Guid.NewGuid()}")
        {
        }

        [Fact]
        public async Task CreateAppointment_EmptyRepository_ShouldReturnCreatedAppointmentId()
        {
            // Arrange
            var command = new Fixture().Build<CreateAppointmentCommand>().Create();
            Guid appointmentId;
            var domainEventPublisherSpy = new DomainEventPublisherSpy<AppointmentCreatedDomainEvent>();
            IOverlappedAppointmentsService overlappedAppointmentService = new Mock<IOverlappedAppointmentsService>(MockBehavior.Loose).Object;

            // Act
            await using (var context = await GetDbContext())
            {
                IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();
                var queryHandler = new CreateAppointmentCommandHandler(mapper, new AppointmentWriteRepository(context), overlappedAppointmentService);

                appointmentId = await queryHandler.HandleAsync(command);

                var unitOfWork = new UnitOfWork<AppointmentDomainServiceDbContext>(context, domainEventPublisherSpy);
                await unitOfWork.SaveAsync();
            }

            // Assert
            await using (var context = await GetDbContext())
            {
                var appointment = await context.Appointments.AsNoTracking().FirstOrDefaultAsync();
                appointment.Should().NotBeNull();
                appointment!.Id.Should().Be(appointmentId);
            }

            domainEventPublisherSpy.ShouldPublishNumberOfEvents(1)
                .And.ShouldContain(_ => _.Appointment.Id == appointmentId);
        }

        [Fact]
        public async Task CreateAppointment_ShouldReturnCreatedAppointmentId()
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var duration = TimeSpan.FromHours(1);
            var attendeeIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var appointmentId = Guid.NewGuid();
            var appointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.StartDate, startDate.AddHours(-1))
                .With(_ => _.Duration, TimeSpan.FromHours(1))
                .With(_ => _.IsDeleted, false)
                .Create();

            await AddAsync(appointment);

            var command = new Fixture().Build<CreateAppointmentCommand>()
                .With(_ => _.StartDate, startDate)
                .With(_ => _.Duration, duration)
                .With(_ => _.AttendeeIds, attendeeIds)
                .Create();
            Guid createdAppointmentId;
            var domainEventPublisherSpy = new DomainEventPublisherSpy<AppointmentCreatedDomainEvent>();
            IOverlappedAppointmentsService overlappedAppointmentService = new Mock<IOverlappedAppointmentsService>(MockBehavior.Loose).Object;

            // Act
            using (var context = await GetDbContext())
            {
                IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();
                var queryHandler = new CreateAppointmentCommandHandler(mapper, new AppointmentWriteRepository(context), overlappedAppointmentService);

                createdAppointmentId = await queryHandler.HandleAsync(command);

                var unitOfWork = new UnitOfWork<AppointmentDomainServiceDbContext>(context, domainEventPublisherSpy);
                await unitOfWork.SaveAsync();
            }

            // Assert
            using (var context = await GetDbContext())
            {
                var createdAppointment = await context.Appointments.AsNoTracking().FirstOrDefaultAsync(_ => _.Id != appointmentId);
                createdAppointment.Should().NotBeNull();
                createdAppointment!.Id.Should().Be(createdAppointmentId);
            }

            domainEventPublisherSpy.ShouldPublishNumberOfEvents(1);
        }

        [Fact]
        public async Task CreateAppointment_WithOverlappedAppointment_ShouldThrowAppointmentOverlappedException()
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var duration = TimeSpan.FromHours(1);
            var attendeeId = Guid.NewGuid();
            var appointmentId = Guid.NewGuid();
            var appointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.StartDate, startDate)
                .With(_ => _.Duration, duration)
                .With(_ => _.Status, AppointmentStatus.Opened)
                .With(_ => _.IsDeleted, false)
                .With(_ => _.Attendees, new List<AppointmentAttendee>
                {
                    new Fixture().Build<AppointmentAttendee>().With(_ => _.UserId, attendeeId).With(_ => _.IsDeleted, false).Create(),
                    new Fixture().Build<AppointmentAttendee>().With(_ => _.UserId, Guid.NewGuid).With(_ => _.IsDeleted, false).Create(),
                })
                .Create();

            await AddAsync(appointment);

            var command = new Fixture().Build<CreateAppointmentCommand>()
                .With(_ => _.StartDate, startDate)
                .With(_ => _.Duration, duration)
                .With(_ => _.AttendeeIds, new[] { attendeeId, Guid.NewGuid() })
                .Create();
            var domainEventPublisherSpy = new DomainEventPublisherSpy<AppointmentCreatedDomainEvent>();
            await using var context = await GetDbContext();
            IOverlappedAppointmentsService overlappedAppointmentService = new OverlappedAppointmentsService(new AppointmentReadRepository(context));

            // Act
            IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();
            var queryHandler = new CreateAppointmentCommandHandler(mapper, new AppointmentWriteRepository(context), overlappedAppointmentService);

            Func<Task<Guid>> func = () => queryHandler.HandleAsync(command);

            await func.Should().ThrowAsync<AppointmentOverlappedException>()
                .Where(ex => ex.OverlappedAppointments != null)
                .Where(ex => ex.OverlappedAppointments!.Contains(appointmentId))
                .Where(ex => ex.Code == AppointmentOverlappedException.ErrorType.Empty.ToErrorCodeString());

            domainEventPublisherSpy.ShouldPublishNumberOfEvents(0);
        }
    }
}
