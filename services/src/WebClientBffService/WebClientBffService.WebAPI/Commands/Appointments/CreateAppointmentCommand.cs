using System;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.WebClientBffService.API.Common;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Commands.Appointments
{
    public record CreateAppointmentCommand : ICommand<Guid>
    {
        public CreateAppointmentCommand(string title, string? description, DateTime startDate, TimeSpan duration, AppointmentType appointmentType, Guid[] attendeeIds)
        {
            Title = title;
            Description = description;
            StartDate = startDate;
            Duration = duration;
            AppointmentType = appointmentType;
            AttendeeIds = attendeeIds;
        }

        public string Title { get; init; }

        public string? Description { get; set; }

        public DateTime StartDate { get; init; }

        public TimeSpan Duration { get; init; }

        public AppointmentType AppointmentType { get; init; }

        public Guid[] AttendeeIds { get; init; }
    }
}
