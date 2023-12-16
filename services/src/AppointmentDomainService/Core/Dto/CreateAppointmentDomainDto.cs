using System;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;

namespace Telemedicine.Services.AppointmentDomainService.Core.Dto
{
    public record CreateAppointmentDomainDto
    {
        public CreateAppointmentDomainDto(string title, string? description, DateTime startDate, TimeSpan duration, AppointmentType type, Guid creatorId, Guid[] attendeeIds)
        {
            Title = title;
            Description = description;
            StartDate = startDate;
            Duration = duration;
            Type = type;
            CreatorId = creatorId;
            AttendeeIds = attendeeIds;
        }

        public string Title { get; init; }

        public string? Description { get; init; }

        public DateTime StartDate { get; init; }

        public TimeSpan Duration { get; init; }

        public AppointmentType Type { get; init; }

        public Guid CreatorId { get; init; }

        public Guid[] AttendeeIds { get; init; }
    }
}
