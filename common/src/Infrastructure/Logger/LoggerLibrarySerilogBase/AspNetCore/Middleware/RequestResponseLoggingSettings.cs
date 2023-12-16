namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.Middleware
{
    /// <summary>
    /// Settings for <see cref="RequestResponseLoggingSettings"/>
    /// </summary>
    public class RequestResponseLoggingSettings
    {
        /// <summary>
        /// If <para>false</para> middleware will turn off.
        /// </summary>
        public bool IsEnabled { get; init; }

        /// <summary>
        /// Describe if need to log request headers
        /// </summary>
        public bool IsLogRequestHeader { get; init; }

        /// <summary>
        /// Describe if need to log request body
        /// </summary>
        public bool IsLogRequestBody { get; init; }

        /// <summary>
        /// Describe if need to log response
        /// </summary>
        public bool IsLogResponse { get; init; }
    }
}
