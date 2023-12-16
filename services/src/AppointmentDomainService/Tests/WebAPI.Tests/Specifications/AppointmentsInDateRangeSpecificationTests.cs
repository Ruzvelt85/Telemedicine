using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Specifications;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.Specifications
{
    public class AppointmentsInDateRangeSpecificationTests
    {
        public static IEnumerable<object[]> GetAppointmentDataForAppointmentsInDateRangeSpecificationTests()
        {
            yield return new object[]
            {
                new [] { new DateTime(2021, 09, 30), new DateTime(2021, 10, 1), new DateTime(2021, 10, 15), new DateTime(2021, 10, 31), new DateTime(2021, 11, 1) },
                new Range<DateTime?>(new DateTime(2021, 10, 1), new DateTime(2021, 11, 01)),
                3
            };
            yield return new object[]
            {
                new [] { new DateTime(2021, 09, 30, 23, 59, 59), new DateTime(2021, 10, 1, 0, 0, 0), new DateTime(2021, 10, 15, 10, 0, 0), new DateTime(2021, 10, 31, 23, 59, 59), new DateTime(2021, 11, 1, 0, 0, 0) },
                new Range<DateTime?>(new DateTime(2021, 10, 1, 0, 0, 0), new DateTime(2021, 11, 1, 0, 0, 0)),
                3
            };
            yield return new object[]
            {
                new [] { new DateTime(2021, 09, 30, 23, 59, 59), new DateTime(2021, 10, 1, 0, 0, 0), new DateTime(2021, 10, 15, 10, 0, 0), new DateTime(2021, 10, 31, 23, 59, 59), new DateTime(2021, 11, 1, 0, 0, 0) },
                new Range<DateTime?>(new DateTime(2021, 10, 1, 0, 0, 0), null),
                4
            };
            yield return new object[]
            {
                new [] { new DateTime(2021, 09, 30, 23, 59, 59), new DateTime(2021, 10, 1, 0, 0, 0), new DateTime(2021, 10, 15, 10, 0, 0), new DateTime(2021, 10, 31, 23, 59, 59), new DateTime(2021, 11, 1, 0, 0, 0) },
                new Range<DateTime?>(null, new DateTime(2021, 11, 1, 0, 0, 0)),
                4
            };
            yield return new object[]
            {
                new [] { new DateTime(2021, 09, 30, 23, 59, 59), new DateTime(2021, 10, 1, 0, 0, 0), new DateTime(2021, 10, 15, 10, 0, 0), new DateTime(2021, 10, 31, 23, 59, 59), new DateTime(2021, 11, 1, 0, 0, 0) },
                new Range<DateTime?>(null, null),
                5
            };
            yield return new object[]
            {
                Array.Empty<DateTime>(),
                new Range<DateTime?>(new DateTime(2021, 10, 1), new DateTime(2021, 11, 01)),
                0
            };
            yield return new object[]
            {
                Array.Empty<DateTime>(),
                new Range<DateTime?>(null, null),
                0
            };
        }

        [Theory]
        [MemberData(nameof(GetAppointmentDataForAppointmentsInDateRangeSpecificationTests))]
        public void AppointmentsInDateRangeSpecificationTest(DateTime[] startDates, Range<DateTime?> rangeFilter, int foundQuantity)
        {
            var appointments = startDates.Select(startDateTime =>
                    new Fixture().Build<Appointment>()
                        .With(e => e.StartDate, startDateTime)
                        .With(e => e.IsDeleted, false)
                        .Create())
                .AsQueryable();

            var specification = new AppointmentsInDateRangeSpecification(rangeFilter);

            var result = appointments.Where(specification).ToList();

            Assert.Equal(foundQuantity, result.Count);
        }
    }
}
