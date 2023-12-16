using Telemedicine.Common.Infrastructure.EntityBase;
using MediatR;

namespace Telemedicine.Common.Infrastructure.DomainEventInfrastructure
{
    /// <summary>
    /// Wrapper for a domain event
    /// </summary>
    public record DomainEventNotification<TEvent>(TEvent DomainEvent)
        : INotification where TEvent : DomainEvent;
}
