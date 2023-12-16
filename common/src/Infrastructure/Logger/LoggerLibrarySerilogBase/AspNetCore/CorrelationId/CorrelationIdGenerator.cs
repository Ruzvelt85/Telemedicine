using System;

namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.CorrelationId
{
    /// <inheritdoc cref="ICorrelationIdGenerator"/>
    public class CorrelationIdGenerator : ICorrelationIdGenerator
    {
        public string Generate()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
