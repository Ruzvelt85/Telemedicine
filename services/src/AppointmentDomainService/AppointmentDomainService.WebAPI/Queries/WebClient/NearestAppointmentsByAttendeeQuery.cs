using System;
using Telemedicine.Common.Infrastructure.Patterns.Query;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Queries.WebClient
{
    public record NearestAppointmentsByAttendeeQuery : IQuery
    {
        public NearestAppointmentsByAttendeeQuery(Guid attendeeId)
        {
            AttendeeId = attendeeId;
        }

        public Guid AttendeeId { get; init; }
    }
}
