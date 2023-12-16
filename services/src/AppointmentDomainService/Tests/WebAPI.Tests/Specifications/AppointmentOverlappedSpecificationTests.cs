using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Specifications;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.Specifications
{
    public class AppointmentOverlappedSpecificationTests
    {
        private static readonly DateTime DefaultDateTime = DateTime.UtcNow.AddYears(1);
        private static readonly Guid Attendee1 = Guid.NewGuid();
        private static readonly Guid Attendee2 = Guid.NewGuid();
        private static readonly Guid Attendee3 = Guid.NewGuid();

        public static IEnumerable<object[]> GetDataForAppointmentsOverlappedSpecificationTests()
        {
            yield return new object[]
            {
                "Opened Appointment with full period and attendee overlap",
                (DefaultDateTime, TimeSpan.FromHours(1), AppointmentStatus.Opened, new [] { Attendee1, Attendee2 }), // 1 ---######---
                (DefaultDateTime, TimeSpan.FromHours(1), new [] { Attendee1 }),                                      // 2 ---######---
                true
            };
            yield return new object[]
            {
                "Opened Appointment with period overlap (period overlaps only second part of appointment) and attendee",
                (DefaultDateTime, TimeSpan.FromHours(1), AppointmentStatus.Opened, new [] { Attendee1, Attendee2 }), // 1 ---######------
                (DefaultDateTime.AddMinutes(30), TimeSpan.FromHours(1), new [] { Attendee2 }),                       // 2 ------######---
                true
            };
            yield return new object[]
            {
                "Opened Appointment with period overlap (period overlaps only first part of appointment) and attendee",
                (DefaultDateTime, TimeSpan.FromHours(1), AppointmentStatus.Opened, new [] { Attendee1, Attendee2 }), // 1 ------######---
                (DefaultDateTime.AddMinutes(-30), TimeSpan.FromHours(1), new [] { Attendee1, Attendee2 }),           // 2 ---######------
                true
            };
            yield return new object[]
            {
                "Opened Appointment with period overlap (contains period) and attendee",
                (DefaultDateTime, TimeSpan.FromHours(1), AppointmentStatus.Opened, new [] { Attendee1, Attendee2 }), // 1 ---########---
                (DefaultDateTime.AddMinutes(10), TimeSpan.FromMinutes(40), new [] { Attendee1, Attendee3 }),         // 2 ----######----
                true
            };
            yield return new object[]
            {
                "Opened Appointment with period overlap (includes in period) and attendee",
                (DefaultDateTime, TimeSpan.FromHours(1), AppointmentStatus.Opened, new [] { Attendee1, Attendee2 }), // 1 ----######----
                (DefaultDateTime.AddMinutes(-10), TimeSpan.FromMinutes(70), new [] { Attendee2, Attendee3 }),        // 2 ---########---
                true
            };
            yield return new object[]
            {
                "Opened Appointment without period overlap (before period) and attendee",
                (DefaultDateTime, TimeSpan.FromHours(1), AppointmentStatus.Opened, new [] { Attendee1, Attendee2 }), // 1 -######-------
                (DefaultDateTime.AddHours(1), TimeSpan.FromHours(1), new [] { Attendee1 }),                          // 2 -------######-
                false
            };
            yield return new object[]
            {
                "Opened Appointment without period overlap (after period) and attendee",
                (DefaultDateTime, TimeSpan.FromHours(1), AppointmentStatus.Opened, new [] { Attendee1, Attendee2 }), // 1 -------######-
                (DefaultDateTime.AddHours(-1), TimeSpan.FromHours(1), new [] { Attendee1 }),                         // 2 -######-------
                false
            };
            yield return new object[]
            {
                "Opened Appointment with full period overlap and without attendee",
                (DefaultDateTime, TimeSpan.FromHours(1), AppointmentStatus.Opened, new [] { Attendee1, Attendee2 }),
                (DefaultDateTime, TimeSpan.FromHours(1), new [] { Attendee3 }),
                false
            };
            yield return new object[]
            {
                "Cancelled Appointment with full period overlap and without attendee",
                (DefaultDateTime, TimeSpan.FromHours(1), AppointmentStatus.Cancelled, new [] { Attendee1, Attendee2 }),
                (DefaultDateTime, TimeSpan.FromHours(1), new [] { Attendee1 }),
                false
            };
            yield return new object[]
            {
                "Done Appointment with full period overlap and without attendee",
                (DefaultDateTime.AddHours(-2), TimeSpan.FromHours(1), AppointmentStatus.Opened, new [] { Attendee1, Attendee2 }),
                (DefaultDateTime, TimeSpan.FromHours(1), new [] { Attendee1 }),
                false
            };
            yield return new object[]
            {
                "Opened Appointment with full period overlap and without attendee (empty attendees in new appointment)",
                (DefaultDateTime, TimeSpan.FromHours(1), AppointmentStatus.Opened, new [] { Attendee1, Attendee2 }),
                (DefaultDateTime, TimeSpan.FromHours(1), Array.Empty<Guid>()),
                false
            };
            yield return new object[]
            {
                "Opened Appointment with full period overlap and without attendee (empty attendees in existed appointment)",
                (DefaultDateTime, TimeSpan.FromHours(1), AppointmentStatus.Opened, Array.Empty<Guid>()),
                (DefaultDateTime, TimeSpan.FromHours(1), new [] { Attendee1 }),
                false
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForAppointmentsOverlappedSpecificationTests))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "<Pending>")]
        public void Test(string msg, (DateTime startDate, TimeSpan duration, AppointmentStatus status, Guid[] attendeeIds) existedAppointment, (DateTime startDate, TimeSpan duration, Guid[] attendeeIds) newAppointment, bool isItemFound)
        {
            // Arrange
            var appointmentAttendees = existedAppointment.attendeeIds.Select(id => new Fixture().Build<AppointmentAttendee>()
                .With(_ => _.UserId, id).With(_ => _.IsDeleted, false).Create()).ToList();

            var appointments = new List<Appointment>
            {
                new Fixture().Build<Appointment>()
                    .With(e => e.StartDate, existedAppointment.startDate)
                    .With(e => e.Duration, existedAppointment.duration)
                    .With(e => e.Status, existedAppointment.status)
                    .With(e => e.Attendees, appointmentAttendees)
                    .With(e => e.IsDeleted, false)
                    .Create()
            }.AsQueryable();

            var specification = new AppointmentOverlappedSpecification(newAppointment.startDate, newAppointment.duration, newAppointment.attendeeIds);

            // Act
            var result = appointments.Where(specification).ToList();

            // Assert
            Assert.Equal(isItemFound, result.Count == 1);
        }
    }
}
