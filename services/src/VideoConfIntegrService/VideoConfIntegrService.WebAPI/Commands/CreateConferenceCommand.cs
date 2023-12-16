using System;
using Telemedicine.Common.Infrastructure.Patterns.Command;

namespace Telemedicine.Services.VideoConfIntegrService.WebAPI.Commands
{
    public record CreateConferenceCommand : ICommand<Guid>
    {
        public Guid AppointmentId { get; init; }

        public string AppointmentTitle { get; init; }

        public DateTime AppointmentStartDate { get; init; }

        public TimeSpan AppointmentDuration { get; init; }

        public CreateConferenceCommand(Guid appointmentId, string appointmentTitle, DateTime appointmentStartDate, TimeSpan appointmentDuration)
        {
            AppointmentId = appointmentId;
            AppointmentTitle = appointmentTitle;
            AppointmentStartDate = appointmentStartDate;
            AppointmentDuration = appointmentDuration;
        }
    }
}
