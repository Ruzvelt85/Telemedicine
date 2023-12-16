using System;

namespace Telemedicine.Common.ServiceInfrastructure.Tests.StartupTests.Setup
{
    public record TestServiceApiResponseDto
    {
        public string? EntityName { get; init; }

        public DateTime? CreatedOn { get; init; }
    }
}
