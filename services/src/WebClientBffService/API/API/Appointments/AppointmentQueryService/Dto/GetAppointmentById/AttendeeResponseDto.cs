using System;
using Telemedicine.Services.WebClientBffService.API.Common;

namespace Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentById
{
    public record AttendeeResponseDto
    {
        public AttendeeResponseDto(Guid id, string firstName, string lastName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
        }

        public Guid Id { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }

        public UserType UserType { get; init; }
    }
}
