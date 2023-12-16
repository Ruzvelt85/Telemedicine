using System;
using System.Collections.Generic;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentQueryService.Dto
{
    public record AppointmentByIdResponseDto
    {
        public AppointmentByIdResponseDto(Guid id, string title, string? description, DateTime startDate, TimeSpan duration, AppointmentType type, AppointmentState state, int rating)
        {
            Id = id;
            Title = title;
            Description = description;
            StartDate = startDate;
            Duration = duration;
            Type = type;
            State = state;
            Rating = rating;
            Attendees = new List<Guid>();
        }

        public Guid Id { get; init; }

        public string Title { get; init; }

        public string? Description { get; init; }

        public DateTime StartDate { get; init; }

        public TimeSpan Duration { get; init; }

        public AppointmentType Type { get; init; }

        public AppointmentState State { get; init; }

        public int Rating { get; init; }

        public IReadOnlyCollection<Guid> Attendees { get; init; }
    }
}
