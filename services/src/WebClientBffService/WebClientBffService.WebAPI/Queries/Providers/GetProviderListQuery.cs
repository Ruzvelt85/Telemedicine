using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.WebClientBffService.API.Providers.ProviderQueryService.Dto;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Queries.Providers
{
    public record GetProviderListQuery : IQuery
    {
        public GetProviderListQuery(ProviderListFilterRequestDto filter, PagingRequestDto paging, SortingType firstNameSortingType)
        {
            Filter = filter;
            Paging = paging;
            FirstNameSortingType = firstNameSortingType;
        }

        public ProviderListFilterRequestDto Filter { get; init; }

        public PagingRequestDto Paging { get; init; }

        public SortingType FirstNameSortingType { get; init; }
    }
}
