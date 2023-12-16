using System;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto
{
    public record CreateAppointmentRequestDto
    {
        public string Title { get; init; } = string.Empty;

        public string? Description { get; init; }

        public DateTime StartDate { get; init; }

        public TimeSpan Duration { get; init; }

        public AppointmentType AppointmentType { get; init; }

        public Guid CreatorId { get; init; }

        public Guid[] AttendeeIds { get; init; } = Array.Empty<Guid>();
    }
}
