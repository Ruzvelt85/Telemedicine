using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;
using Telemedicine.Common.Contracts.GlobalContracts.Utilities;
using Telemedicine.Common.Infrastructure.EntityBase;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;
using Telemedicine.Services.AppointmentDomainService.Core.Events;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.Domain
{
    public class CancelAppointmentTests
    {
        [Theory]
        [InlineData(3600, 3600, AppointmentStatus.Opened, AppointmentStatus.Cancelled, "Cancel appointment before start should have cancelled status", true)]
        [InlineData(-600, 3600, AppointmentStatus.Opened, AppointmentStatus.Missed, "Cancel already ongoing appointment should have missed status", true)]
        [InlineData(-3600, 600, AppointmentStatus.Opened, AppointmentStatus.Missed, "Cancel already done appointment should have missed status", true)]
        [InlineData(-3600, 600, AppointmentStatus.Missed, AppointmentStatus.Missed, "Cancel already missed appointment should have missed status", false)]
        [InlineData(0, 3600, AppointmentStatus.Cancelled, AppointmentStatus.Cancelled, "Cancel already cancelled should have cancelled status", false)]
        public void CancelAppointment_ShouldHaveCorrectStatusAfterCancel(int timeBeforeStartInSec, int durationInSec,
            AppointmentStatus currentStatus, AppointmentStatus expectedStatus, string userMessage, bool expectedCancellationResult)
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var newAppointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.StartDate, DateTime.UtcNow.AddSeconds(timeBeforeStartInSec))
                .With(_ => _.Duration, TimeSpan.FromSeconds(durationInSec))
                .With(_ => _.Status, currentStatus)
                .With(_ => _.IsDeleted, false)
                .Create();
            const string cancelReason = "Some reason";
            var expectedEvent = new AppointmentCancelledDomainEvent(newAppointment);

            // Act
            bool actualCancelResult = newAppointment.Cancel(cancelReason);

            // Assert
            Assert.Equal(expectedCancellationResult, actualCancelResult);
            Assert.True(expectedStatus == newAppointment.Status, userMessage);
            // if status changed, need to check CancelReason
            if (currentStatus != expectedStatus)
            { Assert.Equal(cancelReason, newAppointment.CancelReason); }

            int expectedCountOfEvents = expectedCancellationResult ? 1 : 0;
            newAppointment.DomainEvents.Should()
                .NotBeNull().And
                .HaveCount(expectedCountOfEvents);

            if (expectedCancellationResult)
            { Assert.Equal(expectedEvent, newAppointment.DomainEvents.Single()); }
        }

        [Fact]
        public void Cancel_DeletedAppointment_ShouldThrowEntityAlreadyDeletedException()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var appointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.IsDeleted, true)
                .Create();
            var expectedException = new EntityAlreadyDeletedException(typeof(Appointment), appointmentId);
            var expectedDomainEventCount = appointment.DomainEvents.Count;

            // Act
            Action act = () => appointment.Cancel("Some reason");

            // Assert
            act.Should().Throw<EntityAlreadyDeletedException>()
                .Where(e => e.Type == typeof(Appointment).FullName
                            && e.Id == appointmentId
                            && e.Code == BusinessException.ErrorType.EntityAlreadyDeleted.ToErrorCodeString());
            appointment.DomainEvents.Count.Should().Be(expectedDomainEventCount);
        }
    }
}
