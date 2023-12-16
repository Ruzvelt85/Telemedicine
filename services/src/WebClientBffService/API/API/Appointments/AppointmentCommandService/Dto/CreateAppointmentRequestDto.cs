using System;
using Telemedicine.Services.WebClientBffService.API.Common;

namespace Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentCommandService.Dto
{
    public record CreateAppointmentRequestDto
    {
        public string? Title { get; init; }

        public string? Description { get; init; }

        public DateTime StartDate { get; init; }

        public TimeSpan Duration { get; init; }

        public AppointmentType AppointmentType { get; init; }

        public Guid[]? AttendeeIds { get; init; }
    }
}
