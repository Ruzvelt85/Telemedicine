using System;

namespace Telemedicine.Services.WebClientBffService.API.Common
{
    public record HealthCenterResponseDto
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string State { get; init; } = string.Empty;
    }
}
