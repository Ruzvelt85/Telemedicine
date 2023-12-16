using System;
using System.Collections.Generic;
using Telemedicine.Services.WebClientBffService.API.Common;

namespace Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentById
{
    public record AppointmentByIdResponseDto
    {
        public AppointmentByIdResponseDto(Guid id, string title, string description, DateTime startDate, TimeSpan duration, AppointmentType type, AppointmentStatus status, int rating)
        {
            Id = id;
            Title = title;
            Description = description;
            StartDate = startDate;
            Duration = duration;
            Type = type;
            Status = status;
            Rating = rating;
            Attendees = new List<AttendeeResponseDto>();
        }

        public Guid Id { get; init; }

        public string Title { get; init; }

        public string Description { get; init; }

        public DateTime StartDate { get; init; }

        public TimeSpan Duration { get; init; }

        public AppointmentType Type { get; init; }

        public AppointmentStatus Status { get; init; }

        public int Rating { get; init; }

        public IReadOnlyCollection<AttendeeResponseDto> Attendees { get; init; }
    }
}
