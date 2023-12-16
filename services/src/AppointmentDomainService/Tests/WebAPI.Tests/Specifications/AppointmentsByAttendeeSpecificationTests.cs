using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Specifications;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.Specifications
{
    public class AppointmentsByAttendeeSpecificationTests
    {
        public static IEnumerable<object[]> GetAppointmentDataForAppointmentsByAttendeeSpecificationTests()
        {
            yield return new object[]
            {
                "Appointment with no attendees",
                Guid.Parse("C13DCB72-4031-4C21-B478-45081F615AB9"),
                Array.Empty<Guid>(),
                Guid.Parse("0289D371-C4D5-4CC1-ABE2-42B0B9341A8B"),
                false
            };
            yield return new object[]
            {
                "Appointment with another attendees",
                Guid.Parse("0F807F2C-A7EC-451C-96F6-551F3979D020"),
                new [] { Guid.Parse("CC8A82EA-7B06-415E-95AE-6B421E46A885"), Guid.Parse("0289D371-C4D5-4CC1-ABE2-42B0B9341A8B") },
                Guid.Parse("CFFCAEE0-9B01-4A9C-A108-BD8C062531E4"),
                false
            };
            yield return new object[]
            {
                "Appointment with specified attendee",
                Guid.Parse("8B250063-75C6-4E9C-9D0D-1429B63C3B66"),
                new [] { Guid.Parse("CC8A82EA-7B06-415E-95AE-6B421E46A885"), Guid.Parse("0289D371-C4D5-4CC1-ABE2-42B0B9341A8B") },
                Guid.Parse("0289D371-C4D5-4CC1-ABE2-42B0B9341A8B"),
                true
            };
        }

        [Theory]
        [MemberData(nameof(GetAppointmentDataForAppointmentsByAttendeeSpecificationTests))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "<Pending>")]
        public void AppointmentsByAttendeeSpecificationTest(string message, Guid appointmentId, Guid[] appointmentAttendees, Guid attendeeIdFilter, bool isItemFound)
        {
            var attendees = appointmentAttendees.Select(id => new Fixture()
                .Build<AppointmentAttendee>().With(e => e.AppointmentId, appointmentId)
                .With(e => e.UserId, id)
                .Create()).ToList();

            var appointments = new List<Appointment>
            {
                new Fixture()
                    .Build<Appointment>().With(e => e.Id, appointmentId)
                    .With(e => e.Attendees, attendees).Create()
            }.AsQueryable();

            var specification = new AppointmentsByAttendeeSpecification(attendeeIdFilter);

            var result = appointments.Where(specification).ToList();

            Assert.Equal(isItemFound, result.Count == 1);
        }
    }
}
