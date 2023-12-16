using System;
using System.Collections.Generic;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto
{
    public record DoctorResponseDto
    {
        public Guid Id { get; init; }

        public string InnerId { get; init; } = string.Empty;

        public string FirstName { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public string? PhoneNumber { get; init; }

        public IReadOnlyCollection<HealthCenterResponseDto> HealthCenters { get; init; } = Array.Empty<HealthCenterResponseDto>();
    }
}
