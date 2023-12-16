using JetBrains.Annotations;

namespace Telemedicine.Common.Infrastructure.EntityBase
{
    /// <summary>
    /// Base class for domain event
    /// </summary>
    [PublicAPI]
    public abstract record DomainEvent
    {
        public bool IsPublished { get; set; }
    }
}
