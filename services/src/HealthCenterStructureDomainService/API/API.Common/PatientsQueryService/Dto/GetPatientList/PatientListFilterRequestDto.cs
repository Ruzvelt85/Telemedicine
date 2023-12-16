using System;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientList
{
    public record PatientListFilterRequestDto
    {
        public string? Name { get; init; }

        public Guid DoctorId { get; init; }

        public HealthCenterStructureFilterType HealthCenterStructureFilter { get; init; }
    }
}
