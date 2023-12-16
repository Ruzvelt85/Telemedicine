using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries
{
    public record GetDoctorListQuery : IQuery
    {
        public GetDoctorListQuery(DoctorListFilterRequestDto filter, PagingRequestDto paging, SortingType firstNameSortingType)
        {
            Filter = filter;
            Paging = paging;
            FirstNameSortingType = firstNameSortingType;
        }

        public DoctorListFilterRequestDto Filter { get; init; }

        public PagingRequestDto Paging { get; init; }

        public SortingType FirstNameSortingType { get; init; }

    }
}
