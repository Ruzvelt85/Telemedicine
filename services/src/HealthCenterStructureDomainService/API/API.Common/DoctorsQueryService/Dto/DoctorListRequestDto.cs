using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto
{
    public record DoctorListRequestDto
    {
        public PagingRequestDto Paging { get; init; } = new();

        public DoctorListFilterRequestDto Filter { get; init; } = new();

        public SortingType FirstNameSortingType { get; init; } = SortingType.Asc;
    }
}
