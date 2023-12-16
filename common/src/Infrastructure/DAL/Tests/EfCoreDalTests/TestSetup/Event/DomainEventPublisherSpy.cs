using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Telemedicine.Common.Infrastructure.DomainEventInfrastructure;
using Telemedicine.Common.Infrastructure.EntityBase;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup.Event
{
    public class DomainEventPublisherSpy : IDomainEventPublisher
    {
        private readonly IList<DomainEvent> _publishedDomainEvent = new List<DomainEvent>();

        public Task PublishAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _publishedDomainEvent.Add(domainEvent);
            return Task.CompletedTask;
        }

        public DomainEventPublisherSpy ShouldPublishNumberOfEvents(int number)
        {
            _publishedDomainEvent.Count.Should().Be(number);
            return this;
        }

        public DomainEventPublisherSpy ShouldPublishAllEvents()
        {
            _publishedDomainEvent.Count(_ => !_.IsPublished).Should().Be(0);
            return this;
        }
    }
}
