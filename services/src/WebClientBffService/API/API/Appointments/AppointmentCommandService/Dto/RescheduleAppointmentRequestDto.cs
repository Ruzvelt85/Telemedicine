using System;

namespace Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentCommandService.Dto
{
    public record RescheduleAppointmentRequestDto
    {
        public DateTime StartDate { get; init; }

        public TimeSpan Duration { get; init; }

        public string Reason { get; init; } = string.Empty;
    }
}
