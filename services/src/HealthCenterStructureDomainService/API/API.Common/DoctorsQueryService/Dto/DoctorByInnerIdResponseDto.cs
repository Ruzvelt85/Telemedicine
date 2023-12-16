using System;
using System.Collections.Generic;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto
{
    public record DoctorByInnerIdResponseDto
    {
        public Guid Id { get; init; }

        public string FirstName { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public bool IsAdmin { get; init; }

        public IReadOnlyCollection<HealthCenterResponseDto> HealthCenters { get; init; } = Array.Empty<HealthCenterResponseDto>();
    }
}
