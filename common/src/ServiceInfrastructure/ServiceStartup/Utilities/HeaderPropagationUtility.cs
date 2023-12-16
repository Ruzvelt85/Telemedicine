using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities
{
    public static class HeaderPropagationUtility
    {
        /// <summary>
        /// If <see cref="HeaderPropagationSettings.IsEnabled"/> = true,
        /// adds a middleware that collect headers to be propagated
        /// </summary>
        public static IApplicationBuilder UseHeaderPropagationWithConfiguration(this IApplicationBuilder app, IConfiguration config)
        {
            var settings = config.GetSettings<HeaderPropagationSettings>() ?? HeaderPropagationSettings.Default;
            return settings.IsEnabled ? app.UseHeaderPropagation() : app;
        }

        /// <summary>
        /// If <see cref="HeaderPropagationSettings.IsEnabled"/> = true, adds services required for propagating headers
        /// according to the list of <see cref="HeaderPropagationSettings.InboundHeaders"/>
        /// </summary>
        public static IServiceCollection ConfigureInboundHeaderPropagation(this IServiceCollection services, HeaderPropagationSettings settings)
        {
            if (settings.IsEnabled)
            {
                services.AddHeaderPropagation(options =>
                {
                    if (settings.InboundHeaders != null)
                    {
                        foreach (var header in settings.InboundHeaders)
                        {
                            options.Headers.Add(header.InboundHeaderName, header.CapturedHeaderName);
                        }
                    }
                });
            }
            return services;
        }

        /// <summary>
        /// If <see cref="HeaderPropagationSettings.IsEnabled"/> = true,
        /// adds a message handler for propagating headers collected by the <see cref="Microsoft.AspNetCore.HeaderPropagation.HeaderPropagationMiddleware"/> to a outgoing request
        /// according to the list of <see cref="HeaderPropagationSettings.OutboundHeaders"/>
        /// </summary>
        public static void ConfigureOutboundHeaderPropagation(this IHttpClientBuilder builder, HeaderPropagationSettings settings)
        {
            if (settings.IsEnabled)
            {
                builder.AddHeaderPropagation(options =>
                {
                    if (settings.OutboundHeaders != null)
                    {
                        foreach (var header in settings.OutboundHeaders)
                        {
                            options.Headers.Add(header.CapturedHeaderName, header.OutboundHeaderName);
                        }
                    }
                });
            }
        }
    }
}
