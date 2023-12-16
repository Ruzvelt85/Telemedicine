using System;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsCommandService.Dto
{
    public record CreateOrUpdatePatientRequestDto
    {
        public string InnerId { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public string FirstName { get; init; } = string.Empty;

        public string? PhoneNumber { get; init; }

        public DateTime? BirthDate { get; init; }

        public string HealthCenterInnerId { get; init; } = string.Empty;

        public string? PrimaryCareProviderInnerId { get; init; }

        public bool? IsActive { get; init; }
    }
}
