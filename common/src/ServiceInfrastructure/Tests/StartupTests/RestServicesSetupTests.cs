using System.Collections.Generic;
using System.Linq;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Settings;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Telemedicine.Common.ServiceInfrastructure.Tests.StartupTests
{
    public class RestServicesSetupTests
    {
        [Fact]
        public void RestServiceSettings_AsDictionary_ReadingTest()
        {
            var configuration = new ConfigurationBuilder().SetupConfigurationBuilderForTests().Build();
            var restServiceSettings = RefitConfigureUtility.GetRestServiceSettingsOrDefault(configuration);

            Assert.NotNull(restServiceSettings);
            Assert.Single(restServiceSettings!);

            var (serviceKey, firstServiceSetting) = restServiceSettings.First();
            Assert.Equal("TestServiceKey", serviceKey);

            AssertServiceSetting(firstServiceSetting);
        }

        [Fact]
        public void RestServiceSettings_AsCollection_ReadingTest()
        {
            var configuration = new ConfigurationBuilder().SetupConfigurationBuilderForTests().Build();
            var restServiceSettings = configuration.GetSection("RestServiceSettings_WithCollection").Get<Dictionary<string, RestServiceSettings>>();

            Assert.NotNull(restServiceSettings);
            Assert.Single(restServiceSettings);

            var (serviceKey, firstServiceSetting) = restServiceSettings.First();
            Assert.Equal("0", serviceKey);

            AssertServiceSetting(firstServiceSetting);
        }

        [Fact]
        public void CheckRestServiceRegistrationInIoC()
        {
            IServiceCollection services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().SetupConfigurationBuilderForTests().Build();
            var restServiceSettings = RefitConfigureUtility.GetRestServiceSettingsOrDefault(configuration);

            services.ConfigureRestServices(typeof(RestServicesSetupTests).Assembly, restServiceSettings, new HeaderPropagationSettings());

            var interfaceFullName = restServiceSettings.First().Value.ServiceContract!.Split(',')[0].Trim();
            var registeredService = services.FirstOrDefault(x => x.ServiceType.FullName == interfaceFullName);

            Assert.NotNull(registeredService);
        }

        private static void AssertServiceSetting(RestServiceSettings firstServiceSetting)
        {
            Assert.NotNull(firstServiceSetting);
            Assert.Equal("Test Service Name", firstServiceSetting.Name);
            Assert.NotNull(firstServiceSetting.ServiceContract);
            Assert.Equal("Telemedicine.Common.ServiceInfrastructure.Tests.StartupTests.Setup.ITestServiceApi, Telemedicine.Common.ServiceInfrastructure.Tests.StartupTests", firstServiceSetting.ServiceContract);
            Assert.NotNull(firstServiceSetting.Url);
            Assert.Equal("http://host.docker.internal:5300", firstServiceSetting.Url);
            Assert.True(firstServiceSetting.HealthCheckSettings.IsEnabled);
            Assert.NotNull(firstServiceSetting.HealthCheckSettings.RelativePath);
            Assert.Equal("/health", firstServiceSetting.HealthCheckSettings.RelativePath);
        }
    }
}
