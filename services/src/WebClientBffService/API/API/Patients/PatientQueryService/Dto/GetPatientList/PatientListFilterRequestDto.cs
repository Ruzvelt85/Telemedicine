using Telemedicine.Services.WebClientBffService.API.Common;

namespace Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientList
{
    public record PatientListFilterRequestDto
    {
        public string? Name { get; init; }

        public HealthCenterStructureFilterType HealthCenterStructureFilter { get; init; }
    }
}
