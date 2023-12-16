using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Xunit;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications.Doctor;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.Specifications
{
    public class DoctorByHealthCenterSpecificationTests
    {
        private static IEnumerable<object[]> GetTestData()
        {
            yield return new object[]
            {
                new[] { Guid.Parse("D3058284-59D5-454C-9432-02334793A2E1"), Guid.Parse("C6604811-085E-48D6-81BF-6E908BB45545"), Guid.Parse("9A794CB8-4DCE-4737-9536-40E1AB11262B") },
                new[] { Guid.Parse("C6604811-085E-48D6-81BF-6E908BB45545") },
                true
            };
            yield return new object[]
            {
                new[] { Guid.Parse("D3058284-59D5-454C-9432-02334793A2E1"), Guid.Parse("C6604811-085E-48D6-81BF-6E908BB45545"), Guid.Parse("9A794CB8-4DCE-4737-9536-40E1AB11262B") },
                new[] { Guid.Parse("D3058284-59D5-454C-9432-02334793A2E1"), Guid.Parse("C6604811-085E-48D6-81BF-6E908BB45545") },
                true
            };
            yield return new object[]
            {
                new[] { Guid.Parse("D3058284-59D5-454C-9432-02334793A2E1"), Guid.Parse("C6604811-085E-48D6-81BF-6E908BB45545"), Guid.Parse("9A794CB8-4DCE-4737-9536-40E1AB11262B") },
                new[] { Guid.Parse("D3058284-59D5-454C-9432-02334793A2E1"), Guid.Parse("E18DCAD1-0102-49FA-8836-ED06BE47D126") },
                true
            };
            yield return new object[]
            {
                new[] { Guid.Parse("D3058284-59D5-454C-9432-02334793A2E1"), Guid.Parse("C6604811-085E-48D6-81BF-6E908BB45545"), Guid.Parse("9A794CB8-4DCE-4737-9536-40E1AB11262B") },
                new[] { Guid.Parse("E18DCAD1-0102-49FA-8836-ED06BE47D126") },
                false
            };
            yield return new object[]
            {
                Array.Empty<Guid>(),
                new[] { Guid.Parse("E18DCAD1-0102-49FA-8836-ED06BE47D126") },
                false
            };
            yield return new object[]
            {
                new[] { Guid.Parse("D3058284-59D5-454C-9432-02334793A2E1"), Guid.Parse("C6604811-085E-48D6-81BF-6E908BB45545"), Guid.Parse("9A794CB8-4DCE-4737-9536-40E1AB11262B") },
                Array.Empty<Guid>(),
                false
            };
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void Tests(Guid[] allHealthCenterIds, Guid[] filterHealthCenterIds, bool isItemFoundExpected)
        {
            var healthCenters = allHealthCenterIds.Select(id => Fixtures.GetHealthCenterComposer(id).Create()).ToList();

            var doctors = new List<Doctor>
            {
                Fixtures.GetDoctorComposer().Create().SetHealthCenters(healthCenters)
            }.AsQueryable();

            var specification = new DoctorByHealthCenterSpecification(filterHealthCenterIds);

            var result = doctors.Where(specification).ToList();

            Assert.Equal(isItemFoundExpected, result.Count == 1);
        }
    }
}
