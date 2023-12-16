using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Specifications;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.Specifications
{
    public class AppointmentsFromLastUpdateSpecificationTests
    {
        private static readonly DateTime UtcNow = DateTime.UtcNow;

        private static IEnumerable<object[]> GetAppointmentTestData()
        {

            yield return new object[]
            {
                "Last update is before than appointment UpdateOn",
                UtcNow, UtcNow.AddHours(-1), true
            };
            yield return new object[]
            {
                "Last update is equal to appointment UpdateOn",
                UtcNow, UtcNow, false
            };
            yield return new object[]
            {
                "Last update is after than appointment UpdateOn",
                UtcNow, UtcNow.AddHours(1), false
            };
        }

        [Theory]
        [MemberData(nameof(GetAppointmentTestData))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "<Pending>")]
        public void Tests(string msg, DateTime appointmentUpdatedOn, DateTime lastUpdate, bool isAppointmentFound)
        {
            var patients = new List<Appointment>
            {
                new Fixture().Build<Appointment>().With(e => e.UpdatedOn, appointmentUpdatedOn).Create()
            }.AsQueryable();
            var patientsByNameSpecification = new AppointmentsFromLastUpdateSpecification(lastUpdate);

            var result = patients.Where(patientsByNameSpecification).ToList();

            Assert.Equal(isAppointmentFound, result.Count == 1);
        }
    }
}
