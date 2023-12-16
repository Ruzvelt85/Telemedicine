using System;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Debugging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.Utilities
{
    /// <summary>
    /// Logging utility class
    /// </summary>
    public static class LoggerUtilities
    {
        /// <summary>
        /// Method to check log in service
        /// </summary>
        public static void CheckLog(ILogger logger)
        {
            var date = DateTime.UtcNow;
            //test logging via ILogger interface
            logger.LogTrace("Test log - ILogger.LogTrace {Date}", date);
            logger.LogDebug("Test log - ILogger.LogDebug {Date}", date);
            logger.LogInformation("Test log - ILogger.LogInformation {Date}", date);
            logger.LogWarning("Test log - ILogger.LogWarning {Date}", date);
            logger.LogError("Test log - ILogger.LogError {Date}", date);
            logger.LogCritical("Test log - ILogger.LogCritical {Date}", date);

            //test Serilog methods direct call
            Log.Logger.Verbose("Test log - Logger.Verbose {Date}", date);
            Log.Logger.Debug("Test log - Logger.Debug {Date}", date);
            Log.Logger.Information("Test log - Logger.Information {Date}", date);
            Log.Logger.Warning("Test log - Logger.Warning {Date}", date);
            Log.Logger.Error("Test log - Logger.Error {Date}", date);
            Log.Logger.Fatal("Test log - Logger.Fatal {Date}", date);

            //test Serilog self-logging
            SelfLog.WriteLine("Test log - Selflog {0}", date);
        }

        /// <summary>
        /// Truncates string for log. If you don't sure in string length, you have to use the method. 
        /// </summary>
        /// <param name="logParameter">parameter for logging</param>
        public static string Truncate(this string logParameter)
        {
            var logger = Log.Logger.ForContext(typeof(LoggerUtilities));

            try
            {
                if (!string.IsNullOrWhiteSpace(logParameter))
                {
                    return logParameter.Substring(0, 50);
                }

                return string.Empty;
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Exception occurred during truncate log parameter");
                return string.Empty;
            }
        }
    }
}
