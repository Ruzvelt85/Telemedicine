using System;
using System.Collections.Generic;
using Telemedicine.Services.WebClientBffService.API.Common;

namespace Telemedicine.Services.WebClientBffService.API.Providers.ProviderQueryService.Dto
{
    public record ProviderResponseDto
    {
        public Guid Id { get; init; }

        public string InnerId { get; init; } = string.Empty;

        public string FirstName { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public string? PhoneNumber { get; init; }

        public IReadOnlyCollection<HealthCenterResponseDto> HealthCenters { get; init; } = Array.Empty<HealthCenterResponseDto>();
    }
}
