using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications.Patient;
using Xunit;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.Specifications
{
    public class PatientAssignedToTeamSpecificationTests
    {
        private static IEnumerable<object[]> GetPatientData()
        {
            yield return new object[]
            {
                "Empty Team array doesn't contain patient with AssignedToTeamId",
                Guid.Parse("1B5F0A28-FBA5-48B2-B71F-6954F2447342"), Array.Empty<Guid>(), false
            };
            yield return new object[]
            {
                "Team doesn't contain patient with AssignedToTeamId",
                Guid.Parse("1B5F0A28-FBA5-48B2-B71F-6954F2447342"), new [] { Guid.Parse("2EC0025C-4C0D-4030-8229-E1C400A0E366") }, false
            };
            yield return new object[]
            {
                "Team contains patient with AssignedToTeamId",
                Guid.Parse("1B5F0A28-FBA5-48B2-B71F-6954F2447342"), new [] { Guid.Parse("1B5F0A28-FBA5-48B2-B71F-6954F2447342") }, true
            };

            yield return new object[]
            {
                "Team contains patient with AssignedToTeamId (not case sensitive)",
                Guid.Parse("1B5F0A28-FBA5-48B2-B71F-6954F2447342"), new [] { Guid.Parse("1b5F0a28-fba5-48b2-b71f-6954f2447342") }, true
            };
        }

        [Theory]
        [MemberData(nameof(GetPatientData))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "<Pending>")]
        public void Tests(string msg, Guid assignedToTeamId, Guid[] idtIds, bool isItemFoundExpected)
        {
            var patients = new List<Patient>
            {
                Fixtures.GetPatientComposer(interdisciplinaryTeamId: assignedToTeamId).Create()
            }.AsQueryable();
            var patientAssignedToTeamSpecification = new PatientAssignedToTeamSpecification(idtIds);

            var result = patients.Where(patientAssignedToTeamSpecification).ToList();

            Assert.Equal(isItemFoundExpected, result.Count == 1);
        }
    }
}
