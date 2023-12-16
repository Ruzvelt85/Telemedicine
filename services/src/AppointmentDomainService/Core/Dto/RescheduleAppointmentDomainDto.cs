using System;
using JetBrains.Annotations;

namespace Telemedicine.Services.AppointmentDomainService.Core.Dto
{
    public record RescheduleAppointmentDomainDto
    {
        [UsedImplicitly]
        public RescheduleAppointmentDomainDto(DateTime startDate, TimeSpan duration, string reason, Guid creatorId)
        {
            StartDate = startDate;
            Duration = duration;
            Reason = reason;
            CreatorId = creatorId;
        }

        public DateTime StartDate { get; init; }

        public TimeSpan Duration { get; init; }

        public string Reason { get; init; }

        public Guid CreatorId { get; init; }
    }
}
