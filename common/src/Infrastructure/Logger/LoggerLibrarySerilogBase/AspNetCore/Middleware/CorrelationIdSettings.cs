namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.Middleware
{
    /// <summary>
    /// Settings for <see cref="CorrelationIdMiddleware"/>
    /// </summary>
    public class CorrelationIdSettings
    {
        /// <summary>
        /// If false the middleware will not execute.
        /// </summary>
        public bool IsEnabled { get; init; }

        /// <summary>
        /// HTTP Header name which contains correlation id
        /// </summary>
        public string HeaderName { get; init; } = "x-correlation-id";

        /// <summary>
        /// If <para>true</para> <see cref="CorrelationIdMiddleware"/> will write correlation identifier to response
        /// </summary>
        public bool IsIncludeInResponse { get; init; } = true;

        /// <summary>
        /// Serilog context property name
        /// </summary>
        public string LogPropertyName { get; init; } = "CorrelationId";

        /// <summary>
        /// Is try to retrieve correlation id from HTTP request
        /// </summary>
        public bool AcceptIncomingHeader { get; init; }
    }
}
