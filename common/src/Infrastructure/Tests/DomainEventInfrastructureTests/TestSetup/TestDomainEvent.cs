using System;
using Telemedicine.Common.Infrastructure.EntityBase;

namespace Telemedicine.Common.Infrastructure.Tests.DomainEventInfrastructureTests.TestSetup
{
    public record TestDomainEvent(Guid Id) : DomainEvent;
}
