using Telemedicine.Common.Infrastructure.Testing;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI;
using Xunit;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests
{
    public class InfrastructureTest
    {
        [Fact]
        public void CommonTests()
        {
            var baseTest = new BaseTests(typeof(Startup).Assembly);
            baseTest.RunTests();
        }
    }
}
