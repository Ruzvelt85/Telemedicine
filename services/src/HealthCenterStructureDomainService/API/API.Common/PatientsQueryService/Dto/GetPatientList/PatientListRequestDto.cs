using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientList
{
    public record PatientListRequestDto
    {
        public PagingRequestDto? Paging { get; init; }

        public PatientListFilterRequestDto? Filter { get; init; }

        public SortingType FirstNameSortingType { get; init; } = SortingType.Asc;
    }
}
