using System.ComponentModel;

namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.Middleware
{
    /// <summary>
    /// Settings for <see cref="LogCurrentUserIdHeaderMiddleware"/>
    /// </summary>
    public record LogCurrentUserIdHeaderSettings
    {
        private const string HeaderNameValueDefault = "x-current-user-id";
        private const string LogPropertyNameValueDefault = "CurrentUserId";

        /// <summary>
        /// If false the middleware will not execute, hence it will not enrich the HTTP request header with current user id
        /// </summary>
        public bool IsEnabled { get; init; }

        /// <summary>
        /// HTTP Header name which contains current user id
        /// </summary>
        [DefaultValue(HeaderNameValueDefault)]
        public string HeaderName { get; init; } = HeaderNameValueDefault;

        /// <summary>
        /// Serilog context property name
        /// </summary>
        [DefaultValue(LogPropertyNameValueDefault)]
        public string LogPropertyName { get; init; } = LogPropertyNameValueDefault;
    }
}
