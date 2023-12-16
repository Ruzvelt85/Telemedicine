using System;
using System.Collections.Generic;

namespace Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Dto
{
    public record AppointmentInfoDto
    {
        public Guid Id { get; init; }

        public DateTime StartDate { get; init; }

        public TimeSpan Duration { get; init; }

        public IReadOnlyCollection<Guid> Attendees { get; init; } = Array.Empty<Guid>();
    }
}
