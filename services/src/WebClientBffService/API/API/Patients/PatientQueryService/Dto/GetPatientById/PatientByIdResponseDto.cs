using System;
using Telemedicine.Services.WebClientBffService.API.Common;

namespace Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientById
{
    public record PatientByIdResponseDto
    {
        public Guid Id { get; init; }

        public string FirstName { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public string? PhoneNumber { get; init; }

        public DateTime? BirthDate { get; init; }

        public HealthCenterResponseDto HealthCenter { get; init; } = new();

        public PrimaryCareProviderResponseDto? PrimaryCareProvider { get; init; } = new();
    }
}
