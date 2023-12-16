using System;
using System.Collections.Generic;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Telemedicine.Common.Infrastructure.EventBus;
using Telemedicine.Common.Infrastructure.EventBus.SqsSender;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Settings;
using static Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities.LogConfiguringUtility;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities
{
    public static class HealthCheckConfigureUtility
    {
        /// <summary>
        /// Relative address of the service health check endpoint
        /// </summary>
        public const string DefaultHealthCheckUri = "/health";

        public static IHealthChecksBuilder ConfigureHealthCheckServices(this IServiceCollection services, bool isUseDatabase, string connectionString,
            bool isUseEventBus, SqsSettings? sqsSettings,
            Dictionary<string, RestServiceSettings> restServiceSettingsDict)
        {
            Log.Information(StartExecutingMessage, nameof(HealthCheckConfigureUtility), nameof(ConfigureHealthCheckServices));

            var healthCheckBuilder = services.AddHealthChecks();

            healthCheckBuilder.ConfigureHealthSelf();
            healthCheckBuilder.ConfigureHealthRestServices(restServiceSettingsDict);
            healthCheckBuilder.ConfigureHealthDatabase(isUseDatabase, connectionString);
            healthCheckBuilder.ConfigureHealthEventBus(isUseEventBus, sqsSettings);

            Log.Information(EndExecutingMessage, nameof(HealthCheckConfigureUtility), nameof(ConfigureHealthCheckServices));
            return healthCheckBuilder;
        }

        /// <summary>
        /// Configure simple check of service
        /// </summary>
        private static void ConfigureHealthSelf(this IHealthChecksBuilder healthCheckBuilder) => healthCheckBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

        /// <summary>
        /// Configure health check of connected REST services
        /// </summary>
        private static void ConfigureHealthRestServices(this IHealthChecksBuilder healthCheckBuilder, Dictionary<string, RestServiceSettings> restServiceSettingsDict)
        {
            Log.Information(StartExecutingMessage, nameof(HealthCheckConfigureUtility), nameof(ConfigureHealthRestServices));
            foreach (var setting in restServiceSettingsDict.Values)
            {
                Log.Information("{ClassName}.{MethodName} Start configuring {Setting}",
                    nameof(HealthCheckConfigureUtility), nameof(ConfigureHealthRestServices), setting);

                if (!setting.HealthCheckSettings.IsEnabled || string.IsNullOrWhiteSpace(setting.Url))
                {
                    continue;
                }

                healthCheckBuilder.AddUrlGroup(new Uri($"{setting.Url}{setting.HealthCheckSettings.RelativePath}"), setting.Name);
                Log.Information("{ClassName}.{MethodName} Successfully configured {SettingName}",
                    nameof(HealthCheckConfigureUtility), nameof(ConfigureHealthRestServices), setting.Name);
            }

            Log.Information(EndExecutingMessage, nameof(HealthCheckConfigureUtility), nameof(ConfigureHealthRestServices));
        }

        /// <summary>
        /// Configure check of database 
        /// </summary>
        private static void ConfigureHealthDatabase(this IHealthChecksBuilder healthCheckBuilder, bool isUseDatabase, string connectionString)
        {
            if (!isUseDatabase)
            { return; }

            Log.Information(StartExecutingMessage, nameof(HealthCheckConfigureUtility), nameof(ConfigureHealthDatabase));

            healthCheckBuilder.AddNpgSql(connectionString);

            Log.Information(EndExecutingMessage, nameof(HealthCheckConfigureUtility), nameof(ConfigureHealthDatabase));
        }

        /// <summary>
        /// Configure check of EventBus
        /// </summary>
        /// <param name="healthCheckBuilder"></param>
        /// <param name="isUseEventBus">if true, the health check will be added, otherwise it will not be added</param>
        /// <param name="settings">configuration for the used EventBus</param>
        private static void ConfigureHealthEventBus(this IHealthChecksBuilder healthCheckBuilder, bool isUseEventBus, SqsSettings? settings)
        {
            if (!isUseEventBus || settings is null)
            { return; }

            Log.Information(StartExecutingMessage, nameof(HealthCheckConfigureUtility), nameof(ConfigureHealthEventBus));

            healthCheckBuilder.AddSqs(settings);

            Log.Information(EndExecutingMessage, nameof(HealthCheckConfigureUtility), nameof(ConfigureHealthEventBus));
        }

        public static void Configure(IApplicationBuilder app)
        {
            Log.Information(StartExecutingMessage, nameof(HealthCheckConfigureUtility), nameof(Configure));

            //To configures HealthCheck, using path http://host/health
            app.UseHealthChecks(HealthCheckConfigureUtility.DefaultHealthCheckUri, new HealthCheckOptions
            {
                //Here we can use filters https://docs.microsoft.com/ru-ru/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-2.2#filter-health-checks
                Predicate = _ => true,

                //By default HealthChecks returns a simple Status Code (200 or 503) without the HealthReport data.
                //If you want that HealthCheck-UI shows the HealthReport data from your HealthCheck you can enable it adding an specific ResponseWriter.
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            Log.Information(EndExecutingMessage, nameof(HealthCheckConfigureUtility), nameof(Configure));
        }
    }
}
