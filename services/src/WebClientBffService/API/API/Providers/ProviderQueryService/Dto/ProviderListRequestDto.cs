using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.WebClientBffService.API.Providers.ProviderQueryService.Dto
{
    public record ProviderListRequestDto
    {
        public PagingRequestDto Paging { get; init; } = new();

        public ProviderListFilterRequestDto Filter { get; init; } = new();

        public SortingType FirstNameSortingType { get; init; } = SortingType.Asc;
    }
}
