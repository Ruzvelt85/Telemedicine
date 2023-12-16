using System;
using System.Collections.Generic;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto
{
    public record AppointmentListFilterRequestDto
    {
        public Range<DateTime?> DateRange { get; init; } = new();

        public Guid AttendeeId { get; init; }

        public IReadOnlyCollection<AppointmentState>? AppointmentStates { get; init; }
    }
}
