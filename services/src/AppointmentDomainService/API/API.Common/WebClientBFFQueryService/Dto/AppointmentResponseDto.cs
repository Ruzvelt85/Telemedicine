using System;
using System.Collections.Generic;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto
{
    public record AppointmentResponseDto
    {
        public Guid Id { get; init; }

        public string Title { get; init; } = string.Empty;

        public DateTime StartDate { get; init; }

        public TimeSpan Duration { get; init; }

        public AppointmentType Type { get; init; }

        public AppointmentState State { get; init; }

        public int Rating { get; init; }

        public IReadOnlyCollection<Guid> Attendees { get; init; } = Array.Empty<Guid>();
    }
}
