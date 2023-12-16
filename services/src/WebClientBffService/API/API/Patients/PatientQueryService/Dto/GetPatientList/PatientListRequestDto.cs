using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientList
{
    public record PatientListRequestDto
    {
        public PagingRequestDto Paging { get; init; } = new();

        public PatientListFilterRequestDto? Filter { get; init; }

        public SortingType FirstNameSortingType { get; init; } = SortingType.Asc;
    }
}
