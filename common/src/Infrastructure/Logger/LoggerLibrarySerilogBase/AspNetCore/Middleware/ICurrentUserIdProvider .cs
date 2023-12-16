using System;

namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.Middleware
{
    /// <summary>
    /// Interface using as additional abstraction layer in <see cref="LogCurrentUserIdHeaderMiddleware"/> for getting current user id from jwt token
    /// </summary>
    public interface ICurrentUserIdProvider
    {
        /// <summary>
        /// Get current user id from jwt token
        /// </summary>
        Guid? GetId();
    }
}
