using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using AutoFixture;
using Telemedicine.Services.VideoConfIntegrService.Core.Entities;
using Telemedicine.Services.VideoConfIntegrService.WebAPI.Specifications;

namespace Telemedicine.Services.VideoConfIntegrService.Tests.WebAPI.Tests.Specifications
{
    public class ConferenceWithSpecifiedAppointmentIdSpecificationTests
    {
        public static IEnumerable<object[]> GetConferenceData()
        {
            yield return new object[]
            {
                new [] { Guid.NewGuid(), Guid.Parse("359BC382-B5C1-48A3-8147-6F14ECC57A0C"), Guid.NewGuid()},
                Guid.Parse("359BC382-B5C1-48A3-8147-6F14ECC57A0C"),
                true
            };
            yield return new object[]
            {
                new [] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()},
                Guid.Parse("359BC382-B5C1-48A3-8147-6F14ECC57A0C"),
                false
            };
            yield return new object[]
            {
                new [] { Guid.NewGuid(), Guid.Parse("359BC382-B5C1-48A3-8147-6F14ECC57A0C"), Guid.Parse("359BC382-B5C1-48A3-8147-6F14ECC57A0C"), Guid.NewGuid()},
                Guid.Parse("359BC382-B5C1-48A3-8147-6F14ECC57A0C"),
                true
            };
            yield return new object[]
            {
                Array.Empty<Guid>(),
                Guid.Parse("359BC382-B5C1-48A3-8147-6F14ECC57A0C"),
                false
            };
        }

        [Theory]
        [MemberData(nameof(GetConferenceData))]
        public void ConferenceWithSpecifiedAppointmentIdSpecificationTest(Guid[] appointmentIds, Guid appointmentId, bool isFound)
        {
            var conferences = appointmentIds.Select(appId => new Fixture()
                .Build<Conference>().With(e => e.AppointmentId, appId).With(e => e.IsDeleted, false)
                .Create()).AsQueryable();

            var specification = new ConferenceWithSpecifiedAppointmentId(appointmentId);

            var result = conferences.Where(specification).ToList();

            Assert.Equal(isFound, result.Count != 0);
        }
    }
}
