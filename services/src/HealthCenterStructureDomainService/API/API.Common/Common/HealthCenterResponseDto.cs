using System;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common
{
    public record HealthCenterResponseDto
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string UsaState { get; init; } = string.Empty;
    }
}
