using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Specifications;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.Specifications
{
    public class PreviousAppointmentsSpecificationTests
    {
        public static IEnumerable<object[]> GetAppointmentDataForPreviousAppointmentsSpecificationTests()
        {
            yield return new object[]
            {
                "Previous appointments found",
                new [] { DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddDays(3), DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddDays(-3), DateTime.UtcNow.AddMinutes(20),  DateTime.UtcNow.AddMinutes(-20) },
                3
            };
            yield return new object[]
            {
                "Previous appointments not found",
                new [] { DateTime.UtcNow.AddDays(3), DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddMinutes(20) },
                0
            };
            yield return new object[]
            {
                "Previous appointments not found due to empty array",
                Array.Empty<DateTime>(),
                0
            };
        }

        [Theory]
        [MemberData(nameof(GetAppointmentDataForPreviousAppointmentsSpecificationTests))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "<Pending>")]
        public void PreviousAppointmentsSpecificationTest(string message, DateTime[] startDates, int foundQuantity)
        {
            var appointments = startDates.Select(startDateTime => new Fixture().Build<Appointment>()
                .With(e => e.StartDate, startDateTime)
                .With(e => e.Duration, TimeSpan.FromMinutes(45))
                .With(e => e.IsDeleted, false)
                .Create()).AsQueryable();

            var specification = new PreviousAppointmentsSpecification(DateTime.UtcNow);

            var result = appointments.Where(specification).ToList();

            Assert.Equal(foundQuantity, result.Count);
        }
    }
}
