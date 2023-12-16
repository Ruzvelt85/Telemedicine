using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Xunit;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Specifications;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.Tests.Specifications
{
    public class PatientIdSpecTests
    {
        private static readonly Guid _patientId = Guid.NewGuid();

        public static IEnumerable<object[]> GetMoodDataForTest()
        {
            yield return new object[]
            {
                "Empty moods data",
                Array.Empty<Guid>(),
                _patientId,
                0
            };
            yield return new object[]
            {
                "Empty guid filter",
                new []
                {
                    _patientId,
                    _patientId,
                    _patientId
                },
                Guid.Empty,
                0
            };
            yield return new object[]
            {
                "Found no entity",
                new []
                {
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid()
                },
                _patientId,
                0
            };
            yield return new object[]
            {
                "Found only one entity",
                new []
                {
                    Guid.NewGuid(),
                    _patientId,
                    Guid.NewGuid()
                },
                _patientId,
                1
            };
            yield return new object[]
            {
                "Found two entities",
                new []
                {
                    _patientId,
                    _patientId,
                    Guid.NewGuid()
                },
                _patientId,
                2
            };
        }

        [Theory]
        [MemberData(nameof(GetMoodDataForTest))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "<Pending>")]
        public void MoodByPatientIdSpec_Test(string message, Guid[] patientIds,
            Guid filterPatientId, int foundQuantity)
        {
            var moods = patientIds.Select(patientId => new Fixture().Build<MoodMeasurement>()
                .With(e => e.PatientId, patientId)
                .Create()).AsQueryable();

            var specification = new PatientIdSpec(filterPatientId);

            var result = moods.Where(specification).ToList();

            Assert.Equal(foundQuantity, result.Count);
        }
    }
}
