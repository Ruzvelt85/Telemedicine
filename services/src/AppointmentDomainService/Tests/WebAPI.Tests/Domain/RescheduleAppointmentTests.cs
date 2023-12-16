using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using Xunit;
using Telemedicine.Services.AppointmentDomainService.Core.Dto;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;
using Telemedicine.Services.AppointmentDomainService.Core.Events;
using Telemedicine.Services.AppointmentDomainService.Core.Exceptions;
using Telemedicine.Services.AppointmentDomainService.Core.Services;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.Domain
{
    public class RescheduleAppointmentTests
    {
        [Fact]
        public async Task RescheduleAppointment_WhenNoOverlapped_ShouldRescheduleAndRiseRescheduledDomainEvent()
        {
            // Arrange
            IOverlappedAppointmentsService overlappedAppointmentService = new Mock<IOverlappedAppointmentsService>(MockBehavior.Loose).Object;
            var oldAppointmentId = Guid.NewGuid();
            var attendeeIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var createDto = new CreateAppointmentDomainDto("Title", null, DateTime.UtcNow.AddDays(1), TimeSpan.FromHours(1), AppointmentType.Urgent, Guid.NewGuid(), attendeeIds);
            var oldAppointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, oldAppointmentId)
                .With(_ => _.Title, createDto.Title)
                .With(_ => _.Description, createDto.Description)
                .With(_ => _.StartDate, createDto.StartDate)
                .With(_ => _.Status, AppointmentStatus.Opened)
                .With(_ => _.Duration, createDto.Duration)
                .With(_ => _.IsDeleted, false)
                .With(_ => _.CreatorId, createDto.CreatorId)
                .With(_ => _.Attendees, createDto.AttendeeIds.Select(id => new AppointmentAttendee(id)).ToArray())
                .Create();

            var rescheduleDto = new RescheduleAppointmentDomainDto(DateTime.UtcNow.AddDays(2), TimeSpan.FromHours(1), "test", Guid.NewGuid());

            // Act
            var newAppointment = await oldAppointment.Reschedule(rescheduleDto, overlappedAppointmentService);

            // Assert (oldAppointment should not be changed)
            oldAppointment.Title.Should().Be(createDto.Title);
            oldAppointment.Duration.Should().Be(createDto.Duration);
            oldAppointment.StartDate.Should().Be(createDto.StartDate);
            oldAppointment.Duration.Should().Be(createDto.Duration);
            oldAppointment.IsDeleted.Should().Be(false);
            oldAppointment.CancelReason.Should().Be(rescheduleDto.Reason);
            oldAppointment.CreatorId.Should().Be(createDto.CreatorId);
            oldAppointment.GetState().Should().Be(AppointmentState.Cancelled);
            oldAppointment.Attendees.Should().HaveCount(attendeeIds.Length);
            oldAppointment.Attendees.Select(a => a.UserId).Should().Equal(attendeeIds);
            oldAppointment.DomainEvents.Should()
                .NotBeNull().And
                .HaveCount(1);
            var actualCancelledEvent = oldAppointment.DomainEvents.Single();
            var expectedCancelledEvent = new AppointmentCancelledDomainEvent(oldAppointment);
            Assert.Equal(expectedCancelledEvent, actualCancelledEvent);

            newAppointment.Title.Should().Be(createDto.Title);
            newAppointment.StartDate.Should().Be(rescheduleDto.StartDate);
            newAppointment.Duration.Should().Be(rescheduleDto.Duration);
            newAppointment.CancelReason.Should().BeNull();
            newAppointment.CreatorId.Should().Be(rescheduleDto.CreatorId);
            newAppointment.IsDeleted.Should().Be(false);
            newAppointment.GetState().Should().Be(AppointmentState.Opened);
            newAppointment.Attendees.Should().HaveCount(attendeeIds.Length);
            newAppointment.Attendees.Select(a => a.UserId).Should().Equal(attendeeIds);
            newAppointment.DomainEvents.Should()
                .NotBeNull().And
                .HaveCount(1);
            var actualCreatedEvent = newAppointment.DomainEvents.Single();
            var expectedCreatedEvent = new AppointmentCreatedDomainEvent(newAppointment);
            Assert.Equal(actualCreatedEvent, expectedCreatedEvent);
        }

        [Fact]
        public async Task RescheduleAppointment_WhenOverlappedWithItself_ShouldRescheduleAndRiseRescheduledDomainEvent()
        {
            // Arrange
            var oldAppointmentId = Guid.NewGuid();
            var attendeeIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var oldAppStatus = AppointmentStatus.Opened;
            var createDto = new CreateAppointmentDomainDto("Title", null, DateTime.UtcNow.AddDays(1), TimeSpan.FromHours(1), AppointmentType.Urgent, Guid.NewGuid(), attendeeIds);
            var oldAppointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, oldAppointmentId)
                .With(_ => _.Title, createDto.Title)
                .With(_ => _.Description, createDto.Description)
                .With(_ => _.StartDate, createDto.StartDate)
                .With(_ => _.Status, oldAppStatus)
                .With(_ => _.Duration, createDto.Duration)
                .With(_ => _.CancelReason, (string?)null)
                .With(_ => _.IsDeleted, false)
                .With(_ => _.CreatorId, createDto.CreatorId)
                .With(_ => _.Attendees, createDto.AttendeeIds.Select(id => new AppointmentAttendee(id)).ToArray())
                .Create();

            var rescheduleDto = new RescheduleAppointmentDomainDto(DateTime.UtcNow.AddDays(2), TimeSpan.FromHours(1), "test", Guid.NewGuid());

            var mockService = new Mock<IOverlappedAppointmentsService>();
            mockService
                .Setup(_ => _.GetOverlappedAppointments(It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<Guid[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { new Fixture().Build<Appointment>().With(_ => _.Id, oldAppointmentId).Create() });
            IOverlappedAppointmentsService overlappedAppointmentService = mockService.Object;

            // Act
            var newAppointment = await oldAppointment.Reschedule(rescheduleDto, overlappedAppointmentService);

            // Assert (oldAppointment should not be changed)
            oldAppointment.Title.Should().Be(createDto.Title);
            oldAppointment.Duration.Should().Be(createDto.Duration);
            oldAppointment.StartDate.Should().Be(createDto.StartDate);
            oldAppointment.Duration.Should().Be(createDto.Duration);
            oldAppointment.IsDeleted.Should().Be(false);
            oldAppointment.CancelReason.Should().Be(rescheduleDto.Reason);
            oldAppointment.CreatorId.Should().Be(createDto.CreatorId);
            oldAppointment.GetState().Should().Be(AppointmentState.Cancelled);
            oldAppointment.Attendees.Should().HaveCount(attendeeIds.Length);
            oldAppointment.Attendees.Select(a => a.UserId).Should().Equal(attendeeIds);
            oldAppointment.DomainEvents.Should()
                .NotBeNull().And
                .HaveCount(1);
            var actualCancelledEvent = oldAppointment.DomainEvents.Single();
            var expectedCancelledEvent = new AppointmentCancelledDomainEvent(oldAppointment);
            Assert.Equal(expectedCancelledEvent, actualCancelledEvent);

            newAppointment.Title.Should().Be(createDto.Title);
            newAppointment.StartDate.Should().Be(rescheduleDto.StartDate);
            newAppointment.Duration.Should().Be(rescheduleDto.Duration);
            newAppointment.CancelReason.Should().BeNull();
            newAppointment.CreatorId.Should().Be(rescheduleDto.CreatorId);
            newAppointment.IsDeleted.Should().Be(false);
            newAppointment.GetState().Should().Be(AppointmentState.Opened);
            newAppointment.Attendees.Should().HaveCount(attendeeIds.Length);
            newAppointment.Attendees.Select(a => a.UserId).Should().Equal(attendeeIds);
            newAppointment.DomainEvents.Should()
                .NotBeNull().And
                .HaveCount(1);
            var actualCreatedEvent = newAppointment.DomainEvents.Single();
            var expectedCreatedEvent = new AppointmentCreatedDomainEvent(newAppointment);
            Assert.Equal(actualCreatedEvent, expectedCreatedEvent);
        }

        [Fact]
        public async Task RescheduleAppointment_WhenOverlapped_ShouldThrowOverlappedException()
        {
            // Arrange
            var oldAppointmentId = Guid.NewGuid();
            var attendeeIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var oldAppStatus = AppointmentStatus.Opened;
            var createDto = new CreateAppointmentDomainDto("Title", null, DateTime.UtcNow.AddDays(1), TimeSpan.FromHours(1), AppointmentType.Urgent, Guid.NewGuid(), attendeeIds);
            var oldAppointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, oldAppointmentId)
                .With(_ => _.Title, createDto.Title)
                .With(_ => _.Description, createDto.Description)
                .With(_ => _.StartDate, createDto.StartDate)
                .With(_ => _.Status, oldAppStatus)
                .With(_ => _.Duration, createDto.Duration)
                .With(_ => _.CancelReason, (string?)null)
                .With(_ => _.IsDeleted, false)
                .With(_ => _.CreatorId, createDto.CreatorId)
                .With(_ => _.Attendees, createDto.AttendeeIds.Select(id => new AppointmentAttendee(id)).ToArray())
                .Create();

            var rescheduleDto = new RescheduleAppointmentDomainDto(DateTime.UtcNow.AddDays(2), TimeSpan.FromHours(1), "test", Guid.NewGuid());

            var mockService = new Mock<IOverlappedAppointmentsService>();
            mockService
                .Setup(_ => _.GetOverlappedAppointments(It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<Guid[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Appointment[] { new Fixture().Build<Appointment>().Create() });
            IOverlappedAppointmentsService overlappedAppointmentService = mockService.Object;

            // Act
            var ex = await Record.ExceptionAsync(() => oldAppointment.Reschedule(rescheduleDto, overlappedAppointmentService));

            // Assert (oldAppointment should not be changed)
            Assert.NotNull(ex);
            Assert.IsType<Core.Exceptions.AppointmentOverlappedException>(ex);

            oldAppointment.Title.Should().Be(createDto.Title);
            oldAppointment.Duration.Should().Be(createDto.Duration);
            oldAppointment.StartDate.Should().Be(createDto.StartDate);
            oldAppointment.Duration.Should().Be(createDto.Duration);
            oldAppointment.IsDeleted.Should().Be(false);
            oldAppointment.CancelReason.Should().BeNull();
            oldAppointment.CreatorId.Should().Be(createDto.CreatorId);
            oldAppointment.Status.Should().Be(oldAppStatus);
            oldAppointment.Attendees.Should().HaveCount(attendeeIds.Length);
            oldAppointment.Attendees.Select(a => a.UserId).Should().Equal(attendeeIds);
            oldAppointment.DomainEvents.Should()
                .NotBeNull().And
                .HaveCount(0);
        }

        [Fact]
        public async Task RescheduleAppointment_WhenRescheduleInIncorrectState_ShouldThrowInvalidAppointmentStateException()
        {
            // Arrange
            AppointmentState[] invalidStates = Enum.GetValues<AppointmentState>().Except(new[] { AppointmentState.Opened }).ToArray();

            var oldAppointmentId = Guid.NewGuid();
            var attendeeIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
            IOverlappedAppointmentsService overlappedAppointmentService = new Mock<IOverlappedAppointmentsService>(MockBehavior.Loose).Object;
            var createDto = new CreateAppointmentDomainDto("Title", null, DateTime.UtcNow, TimeSpan.FromHours(1), AppointmentType.Urgent, Guid.NewGuid(), attendeeIds);

            foreach (var status in Enum.GetValues<AppointmentStatus>())
            {
                var oldAppointment = new Fixture().Build<Appointment>()
                    .With(_ => _.Id, oldAppointmentId)
                    .With(_ => _.Title, createDto.Title)
                    .With(_ => _.Description, createDto.Description)
                    .With(_ => _.StartDate, createDto.StartDate)
                    .With(_ => _.Status, status)
                    .With(_ => _.Duration, createDto.Duration)
                    .With(_ => _.CancelReason, (string?)null)
                    .With(_ => _.IsDeleted, false)
                    .With(_ => _.CreatorId, createDto.CreatorId)
                    .With(_ => _.Attendees, createDto.AttendeeIds.Select(id => new AppointmentAttendee(id)).ToArray())
                    .Create();

                var rescheduleDto = new RescheduleAppointmentDomainDto(DateTime.UtcNow.AddDays(2), TimeSpan.FromHours(1), "test", Guid.NewGuid());

                //Act
                var ex = await Record.ExceptionAsync(() => oldAppointment.Reschedule(rescheduleDto, overlappedAppointmentService));

                //Assert (oldAppointment should not be changed)
                Assert.NotNull(ex);
                Assert.IsType<InvalidAppointmentStateException>(ex);

                oldAppointment.GetState().Should().BeOneOf(invalidStates);
                oldAppointment.Title.Should().Be(createDto.Title);
                oldAppointment.Duration.Should().Be(createDto.Duration);
                oldAppointment.StartDate.Should().Be(createDto.StartDate);
                oldAppointment.Duration.Should().Be(createDto.Duration);
                oldAppointment.IsDeleted.Should().Be(false);
                oldAppointment.CancelReason.Should().BeNull();
                oldAppointment.CreatorId.Should().Be(createDto.CreatorId);
                oldAppointment.Status.Should().Be(status);
                oldAppointment.Attendees.Should().HaveCount(attendeeIds.Length);
                oldAppointment.Attendees.Select(a => a.UserId).Should().Equal(attendeeIds);
                oldAppointment.DomainEvents.Should()
                    .NotBeNull().And
                    .HaveCount(0);
            }
        }
    }
}
