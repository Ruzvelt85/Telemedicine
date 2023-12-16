using System;

namespace Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentCommandService.Dto
{
    public record CancelAppointmentRequestDto
    {
        public Guid Id { get; init; }

        public string? Reason { get; init; }
    }
}
