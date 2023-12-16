using System;
using JetBrains.Annotations;
using Telemedicine.Common.Infrastructure.Patterns.Command;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Commands
{
    public record RescheduleAppointmentCommand : ICommand<Guid>
    {
        [UsedImplicitly]
        public RescheduleAppointmentCommand(DateTime startDate, TimeSpan duration, string reason, Guid creatorId)
        {
            StartDate = startDate;
            Duration = duration;
            Reason = reason;
            CreatorId = creatorId;
        }

        public Guid Id { get; init; }

        public DateTime StartDate { get; init; }

        public TimeSpan Duration { get; init; }

        public string Reason { get; init; }

        public Guid CreatorId { get; init; }
    }
}
