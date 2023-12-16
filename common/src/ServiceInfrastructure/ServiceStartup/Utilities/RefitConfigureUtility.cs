using System;
using System.Collections.Generic;
using System.Reflection;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.Refit;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities
{
    public static class RefitConfigureUtility
    {
        /// <summary>
        /// Get Refit settings
        /// </summary>
        /// <returns>The new instance of dictionary with settings, empty dictionary if settings absent or empty.</returns>
        public static Dictionary<string, RestServiceSettings> GetRestServiceSettingsOrDefault(IConfiguration configuration)
            => configuration.GetDictionarySettings<RestServiceSettings>() ?? new Dictionary<string, RestServiceSettings>();

        /// <summary>
        /// Register client for REST service
        /// </summary>
        public static void ConfigureRestServices(this IServiceCollection services, Assembly executingAssembly,
            Dictionary<string, RestServiceSettings>? restServiceSettingsDict, HeaderPropagationSettings headerPropagationSettings)
        {
            if (restServiceSettingsDict is null)
            { return; }

            foreach (var setting in restServiceSettingsDict.Values)
            {
                if (!string.IsNullOrWhiteSpace(setting.ServiceContract) && !string.IsNullOrWhiteSpace(setting.Url))
                {
                    Type? restServiceType = Type.GetType(setting.ServiceContract);

                    if (restServiceType != null)
                    {
                        var methodInfo = typeof(RefitConfigureUtility).GetMethod(nameof(ConfigureRefitServiceClient));
                        methodInfo = methodInfo!.MakeGenericMethod(restServiceType);
                        methodInfo.Invoke(null, new object?[] { services, setting.Url, headerPropagationSettings });
                    }
                }
            }
        }

        /// <summary>
        /// Generic method for registration client for Rest Service; must be public
        /// </summary>
        public static void ConfigureRefitServiceClient<T>(IServiceCollection services, string url, HeaderPropagationSettings headerPropagationSettings)
            where T : class
        {
            services.AddRefitClient<T>(RefitDefaultSettings.Settings)
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(url))
                .ConfigureOutboundHeaderPropagation(headerPropagationSettings);
        }
    }
}
