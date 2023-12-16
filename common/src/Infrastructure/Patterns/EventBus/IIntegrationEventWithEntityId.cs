using System;
using JetBrains.Annotations;

namespace Telemedicine.Common.Infrastructure.Patterns.EventBus
{
    /// <summary>
    /// Additional fields to be used with ancestors of <see cref="IntegrationEvent"/>
    /// </summary>
    [PublicAPI]
    public interface IIntegrationEventWithEntityId
    {
        Guid EntityId { get; }
    }
}
