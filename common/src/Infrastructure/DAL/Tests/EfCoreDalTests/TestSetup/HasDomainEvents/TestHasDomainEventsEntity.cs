using System;
using System.Collections.Generic;
using Telemedicine.Common.Infrastructure.EntityBase;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup.HasDomainEvents
{
    public class TestHasDomainEventsEntity : IEntity, IHasDomainEvents
    {
        private readonly List<DomainEvent> _domainEvents = new();

        public Guid Id { get; init; } = Guid.NewGuid();

        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
    }
}
