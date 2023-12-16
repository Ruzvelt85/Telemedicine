using System;
using Telemedicine.Common.Infrastructure.Patterns.Query;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Queries.Appointments
{
    public record GetAppointmentByIdQuery(Guid Id) : IQuery;
}
