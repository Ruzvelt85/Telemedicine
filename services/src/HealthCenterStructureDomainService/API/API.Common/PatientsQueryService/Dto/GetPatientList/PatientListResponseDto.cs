using System.Collections.Generic;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientList
{
    public record PatientListResponseDto
    {
        public PagingResponseDto Paging { get; init; } = null!;

        public IReadOnlyCollection<PatientResponseDto> Items { get; init; } = null!;
    }
}
