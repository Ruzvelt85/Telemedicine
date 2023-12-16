using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Specifications;
using Xunit;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.Tests.Specifications
{
    public class ClientDateRangeTests
    {
        private static readonly DateTime _now = DateTime.UtcNow;

        public static IEnumerable<object[]> GetMoodDataForTest()
        {
            yield return new object[]
            {
                "Empty moods data",
                Array.Empty<DateTime>(),
                _now, _now.AddDays(1),
                0
            };
            yield return new object[]
            {
                "Empty range",
                new []
                {
                    _now.AddDays(2),
                    _now.AddDays(1),
                    _now,
                    _now.AddDays(-2),
                    _now.AddDays(-3)
                },
                null!, null!,
                5
            };
            yield return new object[]
            {
                "Found only one mood",
                new []
                {
                    _now.AddDays(2),
                    _now.AddDays(1),
                    _now,
                    _now.AddDays(-2),
                    _now.AddDays(-3)
                },
                _now, _now.AddDays(1),
                1
            };
            yield return new object[]
            {
                "Found two moods",
                new []
                {
                    _now.AddDays(2),
                    _now.AddDays(1),
                    _now,
                    _now.AddDays(-2),
                    _now.AddDays(-3)
                },
                _now.AddDays(-2), _now.AddDays(1),
                2
            };
            yield return new object[]
            {
                "Range only to specified",
                new []
                {
                    _now.AddDays(2),
                    _now.AddDays(1),
                    _now,
                    _now.AddDays(-2),
                    _now.AddDays(-3)
                },
                null!, _now.AddDays(1),
                3
            };
            yield return new object[]
            {
                "Range only from specified",
                new []
                {
                    _now.AddDays(2),
                    _now.AddDays(1),
                    _now,
                    _now.AddDays(-2),
                    _now.AddDays(-3)
                },
                _now.AddDays(-2), null!,
                4
            };
        }

        [Theory]
        [MemberData(nameof(GetMoodDataForTest))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "<Pending>")]
        public void MoodByClientDateRangeSpec_Test(string message, DateTime[] clientDates,
            DateTime? from, DateTime? to, int foundQuantity)
        {
            var moods = clientDates.Select(clientDate => new Fixture().Build<MoodMeasurement>()
                .With(e => e.ClientDate, clientDate)
                .Create()).AsQueryable();

            var specification = new ClientDateRangeSpec(from, to);

            var result = moods.Where(specification).ToList();

            Assert.Equal(foundQuantity, result.Count);
        }
    }
}
