using System;
using System.Collections.Generic;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsCommandService.Dto
{
    public record CreateOrUpdateDoctorRequestDto
    {
        public string InnerId { get; init; } = string.Empty;

        public string FirstName { get; init; } = string.Empty;

        public string LastName { get; init; } = string.Empty;

        public IReadOnlyCollection<string> HealthCenterInnerIds { get; init; } = Array.Empty<string>();

        public bool? IsActive { get; init; }
    }
}
