using System;
using Telemedicine.Common.Infrastructure.Patterns.Query;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Queries.MobileClient
{
    public record GetChangedAppointmentListQuery : IQuery
    {
        public Guid AttendeeId { get; init; }

        public DateTime LastUpdate { get; init; }
    }
}
