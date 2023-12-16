using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using AutoFixture;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Specifications;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.Specifications
{
    public class ActiveAppointmentsSpecificationTests
    {
        public static IEnumerable<object[]> GetTestCases()
        {
            yield return new object[]
            {
                "Already done appointment",
                DateTime.UtcNow.AddMonths(-1), AppointmentStatus.Opened,
                false
            };
            yield return new object[]
            {
                "Incorrect status appointment",
                DateTime.UtcNow, AppointmentStatus.Default,
                false
            };
            yield return new object[]
            {
                "Currently started appointment",
                DateTime.UtcNow, AppointmentStatus.Opened,
                true
            };
            yield return new object[]
            {
                "Currently ongoing appointment",
                DateTime.UtcNow.AddMinutes(-30), AppointmentStatus.Opened,
                true
            };
            yield return new object[]
            {
                "Opened appointment (planned for 1 days later)",
                DateTime.UtcNow.AddDays(1), AppointmentStatus.Opened,
                true
            };
            yield return new object[]
            {
                "Opened appointment (planned for 1 hour later)",
                DateTime.UtcNow.AddHours(1), AppointmentStatus.Opened,
                true
            };
            yield return new object[]
            {
                "Cancelled appointment",
                DateTime.UtcNow.AddDays(1), AppointmentStatus.Cancelled,
                false
            };
            yield return new object[]
            {
                "Missed appointment",
                DateTime.UtcNow.AddMinutes(-30), AppointmentStatus.Missed,
                false
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCases))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "<Pending>")]
        public void Test(string msg, DateTime startDateTime, AppointmentStatus status, bool isItemFound)
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                GetAppointment(startDateTime, status)
            }.AsQueryable();

            var specification = new ActiveAppointmentsSpecification();

            // Act
            var result = appointments.Where(specification).ToList();

            // Assert
            Assert.Equal(isItemFound, result.Count == 1);
        }

        private static Appointment GetAppointment(DateTime startDateTime, AppointmentStatus status)
        {
            return new Fixture().Build<Appointment>()
                .With(e => e.StartDate, startDateTime)
                .With(e => e.Duration, TimeSpan.FromHours(1))
                .With(e => e.Status, status)
                .With(e => e.IsDeleted, false)
                .Create();
        }
    }
}
