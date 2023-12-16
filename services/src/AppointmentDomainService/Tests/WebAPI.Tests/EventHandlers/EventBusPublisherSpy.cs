using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Telemedicine.Common.Infrastructure.Patterns.EventBus;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.EventHandlers
{
    public class EventBusPublisherSpy<TEvent> : IEventBusPublisher where TEvent : IntegrationEvent
    {
        private readonly IList<TEvent> _publishedIntegrationEvent = new List<TEvent>();

        public EventBusPublisherSpy<TEvent> And => this;

        public Task PublishAsync(IntegrationEvent @event)
        {
            _publishedIntegrationEvent.Add((TEvent)@event);
            return Task.CompletedTask;
        }

        public Task<bool> TryPublishAsync(IntegrationEvent @event)
        {
            _publishedIntegrationEvent.Add((TEvent)@event);
            return Task.FromResult(true);
        }

        public EventBusPublisherSpy<TEvent> ShouldPublishNumberOfEvents(int number)
        {
            _publishedIntegrationEvent.Count.Should().Be(number);
            return this;
        }

        public EventBusPublisherSpy<TEvent> ShouldContain(Func<TEvent, bool> predicate)
        {
            _publishedIntegrationEvent.Should().Contain(_ => predicate(_));
            return this;
        }
    }
}
