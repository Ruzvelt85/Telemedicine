using System;
using Telemedicine.Common.Infrastructure.Patterns.Query;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Queries.MobileClient
{
    public record GetAppointmentListQuery : IQuery
    {
        public Guid AttendeeId { get; init; }
    }
}
