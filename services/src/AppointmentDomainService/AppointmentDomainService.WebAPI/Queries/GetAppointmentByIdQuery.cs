using System;
using Telemedicine.Common.Infrastructure.Patterns.Query;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Queries
{
    public record GetAppointmentByIdQuery(Guid Id) : IQuery;
}
