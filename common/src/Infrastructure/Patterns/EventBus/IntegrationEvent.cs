using System;

namespace Telemedicine.Common.Infrastructure.Patterns.EventBus
{
    /// <summary>
    /// Base class for storing the information about an integration event.
    /// </summary>
    public abstract record IntegrationEvent
    {
        public Guid Id { get; }

        public DateTime CreationDate { get; }

        public string? CorrelationId { get; init; }

        protected IntegrationEvent(string? correlationId)
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            CorrelationId = correlationId;
        }
    }

    /// <summary>
    /// Base class for storing the information about an integration event with generic data.
    /// </summary>
    public abstract record IntegrationEvent<T> : IntegrationEvent
    {
        public T Data { get; init; }

        protected IntegrationEvent(string? correlationId, T data) : base(correlationId)
        {
            Data = data;
        }
    }
}
