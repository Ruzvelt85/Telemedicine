using System;

namespace Telemedicine.Services.WebClientBffService.API.Providers.ProviderQueryService.Dto
{
    public record ProviderListFilterRequestDto
    {
        public Guid[]? HealthCenterIds { get; init; }

        public string? Name { get; init; }
    }
}
