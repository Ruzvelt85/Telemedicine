using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.Specifications
{
    public class AppointmentsByStatusSpecificationTests
    {
        private readonly IQueryable<Appointment> _appointments;
        private static readonly Guid[] _appointmentIds;

        static AppointmentsByStatusSpecificationTests()
        {
            _appointmentIds = Enumerable.Range(0, 13)
                .Select(_ => Guid.NewGuid())
                .ToArray();
        }

        public AppointmentsByStatusSpecificationTests()
        {
            _appointments = GetAppointments()
                .AsQueryable();
        }

        public static IEnumerable<object[]> GetTestCases()
        {
            yield return new object[]
            {
                AppointmentState.Default,
                Array.Empty<Guid>()
            };
            yield return new object[]
            {
                AppointmentState.Opened,
                new [] { _appointmentIds[6], _appointmentIds[7] }
            };
            yield return new object[]
            {
                AppointmentState.Ongoing,
                new [] { _appointmentIds[3], _appointmentIds[4], _appointmentIds[5] }
            };
            yield return new object[]
            {
                AppointmentState.Cancelled,
                new [] { _appointmentIds[8], _appointmentIds[9] }
            };
            yield return new object[]
            {
                AppointmentState.Missed,
                new [] { _appointmentIds[10], _appointmentIds[11], _appointmentIds[12] }
            };
            yield return new object[]
            {
                AppointmentState.Done,
                new [] { _appointmentIds[0], _appointmentIds[1], _appointmentIds[3] }
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCases))]
        public void AppointmentsByStatusSpecificationTest(AppointmentState appointmentStateFilter, params Guid[] expectedIds)
        {
            // Arrange
            var specification = new AppointmentsByStateSpecification(appointmentStateFilter);

            // Act
            var result = _appointments.Where(specification).ToList();

            // Assert
            Assert.Equal(expectedIds.Length, result.Count);
            var actualIds = result.Select(x => x.Id).ToList();
            Assert.Empty(expectedIds.Except(actualIds));
        }

        [Fact]
        public void CompiledExpressionAppointmentsByStatusSpecificationTest()
        {
            // Arrange && Act
            var appointments = GetAppointments();
            var appointmentsLookup = appointments.ToLookup(x => x.GetState(), x => x.Id);

            // Assert
            AssertCollection(appointmentsLookup[AppointmentState.Default].ToArray(), _appointmentIds[2]);
            AssertCollection(appointmentsLookup[AppointmentState.Opened].ToArray(), _appointmentIds[6], _appointmentIds[7]);
            AssertCollection(appointmentsLookup[AppointmentState.Ongoing].ToArray(), _appointmentIds[3], _appointmentIds[4], _appointmentIds[5]);
            AssertCollection(appointmentsLookup[AppointmentState.Cancelled].ToArray(), _appointmentIds[8], _appointmentIds[9]);
            AssertCollection(appointmentsLookup[AppointmentState.Missed].ToArray(), _appointmentIds[10], _appointmentIds[11], _appointmentIds[12]);
            AssertCollection(appointmentsLookup[AppointmentState.Done].ToArray(), _appointmentIds[0], _appointmentIds[1], _appointmentIds[3]);
        }

        private void AssertCollection(Guid[] actualIds, params Guid[] expectedIds)
        {
            Assert.Equal(expectedIds.Length, actualIds.Length);
            Assert.Empty(expectedIds.Except(actualIds));
        }

        private Appointment GetAppointment(Guid appointmentId, DateTime startDateTime, AppointmentStatus status)
        {
            return new Fixture().Build<Appointment>()
                .With(e => e.Id, appointmentId)
                .With(e => e.StartDate, startDateTime)
                .With(e => e.Duration, TimeSpan.FromHours(1))
                .With(e => e.Status, status)
                .Create();
        }

        private List<Appointment> GetAppointments()
        {
            return new List<Appointment>
                {
                    // Already done appointments
                    GetAppointment(_appointmentIds[0], new DateTime(2021, 11, 09), AppointmentStatus.Opened),
                    GetAppointment(_appointmentIds[1], new DateTime(2021, 11, 11), AppointmentStatus.Opened),
                    GetAppointment(_appointmentIds[3], new DateTime(2021, 11, 15), AppointmentStatus.Opened),
                    // Incorrect status
                    GetAppointment(_appointmentIds[2], new DateTime(2021, 11, 15), AppointmentStatus.Default),
                    // Currently ongoing appointments
                    GetAppointment(_appointmentIds[3], DateTime.UtcNow.AddMinutes(0), AppointmentStatus.Opened),
                    GetAppointment(_appointmentIds[4], DateTime.UtcNow.AddMinutes(-24), AppointmentStatus.Opened),
                    GetAppointment(_appointmentIds[5], DateTime.UtcNow.AddMinutes(-30), AppointmentStatus.Opened),
                     // Currently open appointments
                    GetAppointment(_appointmentIds[6], DateTime.UtcNow.AddDays(1), AppointmentStatus.Opened),
                    GetAppointment(_appointmentIds[7], DateTime.UtcNow.AddMonths(1), AppointmentStatus.Opened),
                    // Cancelled appointments
                    GetAppointment(_appointmentIds[8], DateTime.UtcNow.AddDays(1), AppointmentStatus.Cancelled),
                    GetAppointment(_appointmentIds[9], DateTime.UtcNow.AddDays(1), AppointmentStatus.Cancelled),
                    // Missed appointments
                    GetAppointment(_appointmentIds[10], DateTime.UtcNow.AddDays(-1), AppointmentStatus.Missed),
                    GetAppointment(_appointmentIds[11], DateTime.UtcNow.AddDays(-8), AppointmentStatus.Missed),
                    GetAppointment(_appointmentIds[12], DateTime.UtcNow.AddMonths(-1), AppointmentStatus.Missed)
                };
        }
    }
}
