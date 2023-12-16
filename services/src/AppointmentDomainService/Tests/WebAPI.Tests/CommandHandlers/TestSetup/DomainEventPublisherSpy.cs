using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Telemedicine.Common.Infrastructure.DomainEventInfrastructure;
using Telemedicine.Common.Infrastructure.EntityBase;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.CommandHandlers.TestSetup
{
    public class DomainEventPublisherSpy<TEvent> : IDomainEventPublisher where TEvent : DomainEvent
    {
        private readonly IList<TEvent> _publishedDomainEvent = new List<TEvent>();

        public DomainEventPublisherSpy<TEvent> And => this;

        public Task PublishAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _publishedDomainEvent.Add((TEvent)domainEvent);
            return Task.CompletedTask;
        }

        public DomainEventPublisherSpy<TEvent> ShouldPublishNumberOfEvents(int number)
        {
            _publishedDomainEvent.Count(_ => _.IsPublished).Should().Be(number);
            return this;
        }

        public DomainEventPublisherSpy<TEvent> ShouldPublishAllEvents()
        {
            _publishedDomainEvent.Count(_ => !_.IsPublished).Should().Be(0);
            return this;
        }

        public DomainEventPublisherSpy<TEvent> ShouldContain(Func<TEvent, bool> predicate)
        {
            _publishedDomainEvent.Should().Contain(_ => predicate(_));
            return this;
        }
    }
}
