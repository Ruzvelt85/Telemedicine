using System;

namespace Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceCommandService.Dto
{
    public record CreateConferenceRequestDto
    {
        public Guid AppointmentId { get; init; }

        public string? AppointmentTitle { get; init; }

        public DateTime AppointmentStartDate { get; init; }

        public TimeSpan AppointmentDuration { get; init; }
    }
}
