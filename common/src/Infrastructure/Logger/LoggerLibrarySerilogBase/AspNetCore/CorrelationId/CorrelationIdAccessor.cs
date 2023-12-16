namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.CorrelationId
{
    /// <inheritdoc cref="ICorrelationIdAccessor"/>
    public class CorrelationIdAccessor : ICorrelationIdAccessor
    {
        public string? CorrelationId { get; set; }
    }
}
