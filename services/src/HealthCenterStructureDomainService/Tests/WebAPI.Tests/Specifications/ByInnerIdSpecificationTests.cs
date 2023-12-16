using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.Specifications
{
    public class ByInnerIdSpecificationTests
    {
        private static IEnumerable<object[]> GetPatientsData()
        {
            yield return new object[]
            {
                new [] { "25954INV", "37082INV", "41203INV", "17226INV" },
                "37082INV",
                true
            };
            yield return new object[]
            {
                new [] { "25954INV", "37082INV", "41203INV", "17226INV" },
                "71712INV",
                false
            };
            yield return new object[]
            {
                new [] { "25954INV", "37082INV", "41203INV", "17226INV" },
                "37082",
                false
            };
            yield return new object[]
            {
                Array.Empty<string>(),
                "37082INV",
                false
            };
        }

        [Theory]
        [MemberData(nameof(GetPatientsData))]
        public void Tests(string[] innerIds, string filter, bool isItemFoundExpected)
        {
            var entities = innerIds.Select(innerId =>
                new InnerIdTestEntity() { InnerId = innerId }).AsQueryable();

            var entityByInnerIdSpecification = new ByInnerIdSpecification<InnerIdTestEntity>(filter);

            var result = entities.Where(entityByInnerIdSpecification).ToList();

            Assert.Equal(isItemFoundExpected, result.Count == 1);
        }
    }
}
