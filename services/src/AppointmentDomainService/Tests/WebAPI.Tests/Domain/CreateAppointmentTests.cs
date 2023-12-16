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
using Telemedicine.Services.AppointmentDomainService.Core.Services;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.Domain
{
    public class CreateAppointmentTests
    {
        [Fact]
        public async Task CreateNewAppointment_WhenNoOverlapped_ShouldCreateNewAppointmentAndRiseCreatedDomainEvent()
        {
            // Arrange
            DateTime appointmentDate = DateTime.UtcNow.AddDays(1);
            IOverlappedAppointmentsService overlappedAppointmentService = new Mock<IOverlappedAppointmentsService>(MockBehavior.Loose).Object;
            // Act
            var dto = new CreateAppointmentDomainDto("Title", "Description", appointmentDate, TimeSpan.FromMinutes(30), AppointmentType.Annual, Guid.NewGuid(), new[] { Guid.NewGuid(), Guid.NewGuid() });
            var appointment = await Appointment.Create(dto, overlappedAppointmentService);

            // Assert
            var expectedEvent = new AppointmentCreatedDomainEvent(appointment);
            appointment.Should().NotBeNull();
            appointment.Title.Should().Be(dto.Title);
            appointment.Description.Should().Be(dto.Description);
            appointment.StartDate.Should().Be(appointmentDate);
            appointment.Duration.Should().Be(dto.Duration);
            appointment.GetState().Should().Be(AppointmentState.Opened);
            appointment.Attendees.Should().HaveCount(2);
            appointment.DomainEvents.Should()
                .NotBeNull().And
                .HaveCount(1);

            var actualEvent = appointment.DomainEvents.Single();
            Assert.Equal(expectedEvent, actualEvent);
        }

        [Fact]
        public async Task CreateNewAppointment_WhenOverlapped_ShouldThrowOverlappedException()
        {
            // Arrange
            var mockService = new Mock<IOverlappedAppointmentsService>();
            mockService
                .Setup(_ => _.GetOverlappedAppointments(It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<Guid[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Appointment[] { new Fixture().Build<Appointment>().Create() });
            IOverlappedAppointmentsService overlappedAppointmentService = mockService.Object;

            // Act
            var dto = new CreateAppointmentDomainDto("Title", "Description", DateTime.UtcNow.AddDays(1), TimeSpan.FromMinutes(30), AppointmentType.Annual, Guid.NewGuid(), new[] { Guid.NewGuid(), Guid.NewGuid() });
            var ex = await Record.ExceptionAsync(() => Appointment.Create(dto, overlappedAppointmentService));

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<Core.Exceptions.AppointmentOverlappedException>(ex);
        }
    }
}
