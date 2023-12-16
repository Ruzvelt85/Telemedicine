using System;
using Telemedicine.Services.WebClientBffService.API.Common;

namespace Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentList
{
    public record AttendeeResponseDto
    {
        public Guid Id { get; init; }

        public string? FirstName { get; init; }

        public string? LastName { get; init; }

        public UserType UserType { get; init; }

        public DateTime? BirthDate { get; init; }
    }
}
