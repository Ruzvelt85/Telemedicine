using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications.HealthCenter;
using Xunit;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.Specifications
{
    public class HealthCenterHasAnyDoctorSpecificationTests
    {
        private static IEnumerable<object[]> GetTeamsTests()
        {
            yield return new object[]
            {
                "Health Center doesn't contain any doctor",
                Array.Empty<Guid>(), Guid.Parse("9A794CB8-4DCE-4737-9536-40E1AB11262B"), false
            };
            yield return new object[]
            {
                "Health Center doesn't contain any doctor with current DoctorId",
                new[] { Guid.Parse("D3058284-59D5-454C-9432-02334793A2E1") }, Guid.Parse("9A794CB8-4DCE-4737-9536-40E1AB11262B"), false
            };
            yield return new object[]
            {
                "Health Center contains any doctor with current DoctorId",
                new[] { Guid.Parse("D3058284-59D5-454C-9432-02334793A2E1") }, Guid.Parse("D3058284-59D5-454C-9432-02334793A2E1"), true
            };
        }

        [Theory]
        [MemberData(nameof(GetTeamsTests))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "<Pending>")]
        public void Tests(string msg, Guid[] doctorIdsInHealthCenter, Guid doctorFilter, bool isItemFoundExpected)
        {
            var members = doctorIdsInHealthCenter.Select(id => Fixtures.GetDoctorComposer(id).Create()).ToList();
            var healthCenters = new List<HealthCenter>
            {
                Fixtures.GetHealthCenterComposer().With(pc => pc.Doctors, members).Create()
            }.AsQueryable();
            var healthCenterHasAnySpec = new HealthCenterHasAnyDoctorSpecification(doctorFilter);

            var result = healthCenters.Where(healthCenterHasAnySpec).ToList();

            Assert.Equal(isItemFoundExpected, result.Count == 1);
        }
    }
}
