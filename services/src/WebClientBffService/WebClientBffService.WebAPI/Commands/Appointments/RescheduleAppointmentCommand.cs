using System;
using JetBrains.Annotations;
using Telemedicine.Common.Infrastructure.Patterns.Command;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Commands.Appointments
{
    public record RescheduleAppointmentCommand : ICommand<Guid>
    {
        [UsedImplicitly]
        public RescheduleAppointmentCommand(DateTime startDate, TimeSpan duration, string reason)
        {
            StartDate = startDate;
            Duration = duration;
            Reason = reason;
        }

        public Guid Id { get; init; }

        public DateTime StartDate { get; init; }

        public TimeSpan Duration { get; init; }

        public string Reason { get; init; }
    }
}
