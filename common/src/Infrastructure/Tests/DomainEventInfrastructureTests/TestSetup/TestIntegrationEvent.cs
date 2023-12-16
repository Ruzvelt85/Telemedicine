using System;
using Telemedicine.Common.Infrastructure.Patterns.EventBus;

namespace Telemedicine.Common.Infrastructure.Tests.DomainEventInfrastructureTests.TestSetup
{
    public record TestIntegrationEvent(Guid SomeEntityId, string? CorrelationId) : IntegrationEvent(CorrelationId);
}
