using System;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.Refit;
using Microsoft.Extensions.Configuration;
using Refit;

namespace Telemedicine.Common.Infrastructure.IntegrationTesting
{
    public class HttpServiceFixture<TService>
    {
        private readonly string _hostUrl;

        public HttpServiceFixture()
        {
            var configuration = new ConfigurationBuilder().SetupConfigurationBuilderForTests().Build();
            var settings = configuration.GetSettings<IntegrationTestSettings>();

            if (string.IsNullOrWhiteSpace(settings?.HostUrl))
            {
                throw new Exception("Incorrect settings for integration tests (HttpServiceFixture): HostUrl is null");
            }

            _hostUrl = settings.HostUrl;
        }

        public TService GetRestService()
        {
            return GetRestService(new RefitSettings(new NewtonsoftJsonContentSerializer(JsonSerializerUtility.CreateAndConfigureJsonSerializerSettings())));
        }

        public TService GetRestService(RefitSettings settings)
        {
            return RestService.For<TService>(_hostUrl, settings);
        }
    }
}
