using System.Collections.Generic;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.Settings;

namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.Utilities
{
    public static class LoggerSettingsUtilities
    {
        /// <summary>
        /// Check if settings exist for given endpoint parameters
        /// </summary>
        public static bool HasIgnoreLoggingBodySettings(Dictionary<string, IgnoreLoggingResponseBodySettings> logIgnoreSettings, string httpVerb, string path)
        {
            var settings = GetIgnoreLoggingBodySettings(logIgnoreSettings, httpVerb, path);
            return settings != null;
        }

        /// <summary>
        /// Get settings for given endpoint parameters
        /// </summary>
        public static IgnoreLoggingResponseBodySettings? GetIgnoreLoggingBodySettings(Dictionary<string, IgnoreLoggingResponseBodySettings> settings, string httpVerb, string path)
        {
            foreach (var setting in settings)
            {
                if (string.IsNullOrWhiteSpace(setting.Value.HttpVerb) || string.IsNullOrWhiteSpace(setting.Value.Path))
                {
                    continue;
                }

                if (IsMatchingHttpVerb(setting.Value.HttpVerb, httpVerb) && IsMatchingPath(setting.Value.Path, path))
                    return setting.Value;
            }

            return null;
        }

        private static bool IsMatchingHttpVerb(string configHttpVerb, string requestHttpVerb)
        {
            return IsMatchingConfigParam(configHttpVerb, requestHttpVerb);
        }

        private static bool IsMatchingPath(string configPath, string requestPath)
        {
            return IsMatchingConfigParam(configPath, requestPath);
        }

        /// <summary>
        /// Check if parameter from config matches with given value
        /// </summary>
        /// <param name="configValue" example ="GET"></param>
        /// <param name="actualValue"></param>
        /// <example>actualValue can be GET, api/Appointments</example>
        /// <returns></returns>
        private static bool IsMatchingConfigParam(string configValue, string? actualValue)
        {
            return configValue.ToLowerAndTrim() == actualValue?.ToLower();
        }
    }
}
