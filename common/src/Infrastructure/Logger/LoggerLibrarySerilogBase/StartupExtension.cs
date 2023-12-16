using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;

namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase
{
    /// <summary>Extension methods for configuring logs
    /// </summary>
    [PublicAPI]
    public static class StartUpLoggingExtension
    {
        /// <summary>
        /// Configures logging - adding points for logging (stop service, every request), correlation id transfer
        /// </summary>
        /// <param name="app">Provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">Provides information about the web hosting environment an application is running in.</param>
        /// <param name="applicationLifetime">Allows consumers to perform cleanup during a graceful shutdown.</param>
        /// <param name="shutdownLogAction">Logging method called when service stops</param>
        public static IApplicationBuilder ConfigureLogging(this IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env,
            Microsoft.Extensions.Hosting.IHostApplicationLifetime applicationLifetime, Action shutdownLogAction) =>
            Configure(app, env, applicationLifetime, shutdownLogAction);

        /// <summary>
        /// Configures logging - adding points for logging (stop service, every request), correlation id transfer
        /// </summary>
        /// <param name="app">Provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">Provides information about the web hosting environment an application is running in.</param>
        /// <param name="applicationLifetime">Allows consumers to perform cleanup during a graceful shutdown.</param>
        /// <param name="shutdownLogAction">Logging method called when service stops</param>
        public static IApplicationBuilder Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env,
            Microsoft.Extensions.Hosting.IHostApplicationLifetime applicationLifetime, Action shutdownLogAction)
        {
            //Register the method called when the service is disabled
            applicationLifetime.ApplicationStopping.Register(shutdownLogAction);
            return app;
        }
    }
}
