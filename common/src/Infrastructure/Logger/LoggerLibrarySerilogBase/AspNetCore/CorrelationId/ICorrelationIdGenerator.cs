namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.CorrelationId
{
    /// <summary>
    /// Interface to generate correlation id
    /// </summary>
    public interface ICorrelationIdGenerator
    {
        /// <summary>
        /// Generate correlation id
        /// </summary>
        /// <returns>Correlation id</returns>
        public string Generate();
    }
}
