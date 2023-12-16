using System;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto
{
    public record RescheduleAppointmentRequestDto
    {
        public DateTime StartDate { get; init; }

        public TimeSpan Duration { get; init; }

        public string Reason { get; init; } = string.Empty;

        public Guid CreatorId { get; init; }
    }
}
