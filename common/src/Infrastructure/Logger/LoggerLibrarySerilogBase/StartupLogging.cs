using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase
{
    [PublicAPI]
    public class StartupLogging
    {
        public IConfiguration Configuration { get; }

        public ILogger Logger { get; }

        public StartupLogging(IConfiguration configuration)
        {
            Configuration = configuration;
            InitSelfLog();
            Logger = InitSerilog();

            Logger.ForContext<StartupLogging>();
            Logger.Warning("Service is starting up");
        }

#pragma warning disable S2228 // Console logging should not be used
#pragma warning disable S106 // Standard outputs should not be used directly to log anything
        private static void InitSelfLog() =>
            Serilog.Debugging.SelfLog.Enable(msg =>
            {
                var stackTrace = SelfLogUtilities.PrettifyStackTrace(Environment.StackTrace);
                var renderedMessage = SelfLogUtilities.BuildSelflog(msg, stackTrace);

                Debug.WriteLine(renderedMessage);
                Console.WriteLine(renderedMessage);
            });
#pragma warning restore S2228 // Console logging should not be used
#pragma warning restore S106 // Standard outputs should not be used directly to log anything

        private ILogger InitSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();
            return Log.Logger;
        }

        /// <summary>Method called runtime when service stops.
        /// </summary>
        public void OnShutdown() => Logger.Warning("Service is shutting down");
    }
}
