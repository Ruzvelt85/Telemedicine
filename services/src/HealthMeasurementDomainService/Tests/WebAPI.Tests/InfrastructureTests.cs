using Telemedicine.Common.Infrastructure.Testing;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI;
using Xunit;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.Tests
{
    public class InfrastructureTests
    {
        [Fact]
        public void CommonTests()
        {
            var baseTest = new BaseTests(typeof(Startup).Assembly);
            baseTest.RunTests();
        }
    }
}
