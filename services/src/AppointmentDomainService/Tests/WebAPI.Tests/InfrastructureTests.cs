using Telemedicine.Common.Infrastructure.Testing;
using Telemedicine.Services.AppointmentDomainService.WebAPI;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests
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
