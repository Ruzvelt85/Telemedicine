using System;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Commands
{
    public record CreateAppointmentCommand : ICommand<Guid>
    {
        // ReSharper disable once UnusedMember.Global
        public CreateAppointmentCommand(string title, string? description, DateTime startDate, TimeSpan duration, AppointmentType type, Guid creatorId, Guid[] attendeeIds)
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

        public Guid CreatorId { get; set; }

        public Guid[] AttendeeIds { get; init; }
    }
}
