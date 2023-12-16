using System;

namespace Telemedicine.Common.ServiceInfrastructure.Tests.StartupTests.Setup
{
    public record TestServiceApiRequestDto
    {
        public Guid EntityId { get; init; }
    }
}
