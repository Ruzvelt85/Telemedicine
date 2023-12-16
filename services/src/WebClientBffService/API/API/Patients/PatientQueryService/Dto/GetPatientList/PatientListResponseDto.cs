using System.Collections.Generic;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientList
{
    public record PatientListResponseDto
    {
        public PagingResponseDto? Paging { get; init; }

        public IReadOnlyCollection<PatientResponseDto>? Items { get; init; }
    }
}
