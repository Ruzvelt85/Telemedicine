using Telemedicine.Common.Infrastructure.Testing;
using Telemedicine.Services.VideoConfIntegrService.WebAPI;
using Xunit;

namespace Telemedicine.Services.VideoConfIntegrService.Tests.WebAPI.Tests
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
