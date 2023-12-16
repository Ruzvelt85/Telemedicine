namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.CorrelationId
{
    /// <summary>
    /// Interface to access correlation id
    /// </summary>
    public interface ICorrelationIdAccessor
    {
        /// <summary>
        /// Property that represents correlation id
        /// </summary>
        public string? CorrelationId { get; set; }
    }
}
