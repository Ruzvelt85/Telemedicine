using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications;
using Xunit;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.Specifications
{
    public class ByPersonNameSpecificationTests
    {
        private static IEnumerable<object[]> GetPatientData()
        {
            yield return new object[]
            {
                "FirstName starts with filter",
                "Alex", "Black", "Ale", true
            };
            yield return new object[]
            {
                "FirstName starts with filter (case insensitive search)",
                "Alex", "Black", "ale", true
            };
            yield return new object[]
            {
                "LastName starts with filter",
                "Bruno", "Alexandrinho", "Ale", true
            };
            yield return new object[]
            {
                "LastName not starts with filter (case insensitive search)",
                "Bruno", "Alexandrinho", "ale", true
            };
            yield return new object[]
            {
                "FirstName starts with 3 symbols and LastName starts with 1 symbol",
                "Alex", "Black", "Ale B", true
            };
            yield return new object[]
            {
                "LastName starts with 3 symbols and FirstName starts with 1 symbol",
                "Bill", "Alexandro", "Ale B", true
            };
            yield return new object[]
            {
                "Search for only if name starts with (not contains)",
                "Salex", "Black", "ale", false
            };
            yield return new object[]
            {
                "Space at the beginning and ending of filter",
                "Alex", "Black", "   Ale ", true
            };
            yield return new object[]
            {
                "Apostrophe at the beginning",
                "Alex", "Black", "'Ale", false
            };
            yield return new object[]
            {
                "Hyphen at the beginning",
                "Alex", "Black", "-Ale", false
            };
            yield return new object[]
            {
                "More spaces between firstname and lastname",
                "Alex", "Black", "Alex   B", true
            };
        }

        [Theory]
        [MemberData(nameof(GetPatientData))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "<Pending>")]
        public void Tests(string msg, string firstname, string lastname, string filter, bool isItemFoundExpected)
        {
            var patients = new List<Patient>
            {
                Fixtures.GetPatientComposer(firstName: firstname, lastName: lastname).Create()
            }.AsQueryable();
            var patientsByNameSpecification = new ByPersonNameSpecification<Patient>(filter);

            var result = patients.Where(patientsByNameSpecification).ToList();

            Assert.Equal(isItemFoundExpected, result.Count == 1);
        }
    }
}
