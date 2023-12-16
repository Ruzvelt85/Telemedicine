using System;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto
{
    public record ChangedAppointmentListRequestDto
    {
        public ChangedAppointmentListRequestDto() { }

        public ChangedAppointmentListRequestDto(Guid attendeeId, DateTime lastUpdate)
        {
            AttendeeId = attendeeId;
            LastUpdate = lastUpdate;
        }

        public Guid AttendeeId { get; init; }

        public DateTime LastUpdate { get; init; }
    }
}
