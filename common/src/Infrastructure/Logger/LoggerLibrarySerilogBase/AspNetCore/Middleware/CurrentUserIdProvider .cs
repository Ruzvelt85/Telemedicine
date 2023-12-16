using System;

namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.Middleware
{
    /// <summary>
    /// Implementation of ICurrentUserIdProvider. For more info see: <see cref="ICurrentUserIdProvider"/>
    /// </summary>
    public class CurrentUserIdProvider : ICurrentUserIdProvider
    {
        private readonly Guid? _id;

        public CurrentUserIdProvider(Guid? id)
        {
            _id = id;
        }

        public Guid? GetId() => _id;
    }
}
