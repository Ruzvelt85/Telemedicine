using System.Collections.Generic;
using JetBrains.Annotations;

namespace Telemedicine.Common.Infrastructure.EntityBase
{
    /// <summary>
    /// Interface for use with entities that has domain events
    /// </summary>
    [PublicAPI]
    public interface IHasDomainEvents
    {
        public IReadOnlyCollection<DomainEvent> DomainEvents { get; }
    }
}
