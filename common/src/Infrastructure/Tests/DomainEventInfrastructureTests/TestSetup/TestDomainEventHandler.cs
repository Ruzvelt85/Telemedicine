using Telemedicine.Common.Infrastructure.DomainEventInfrastructure;
using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.CorrelationId;
using Telemedicine.Common.Infrastructure.Patterns.EventBus;

namespace Telemedicine.Common.Infrastructure.Tests.DomainEventInfrastructureTests.TestSetup
{
    public class TestDomainEventHandler : BasePublisherEventHandler<TestDomainEvent>
    {
        public TestDomainEventHandler(IEventBusPublisher eventBusPublisher, ICorrelationIdAccessor correlationIdAccessor)
            : base(eventBusPublisher, correlationIdAccessor)
        {
        }

        protected override IntegrationEvent GetIntegrationEvent(TestDomainEvent domainEvent)
        {
            var integrationEvent = new TestIntegrationEvent(domainEvent.Id, CorrelationIdAccessor.CorrelationId);
            return integrationEvent;
        }
    }
}
