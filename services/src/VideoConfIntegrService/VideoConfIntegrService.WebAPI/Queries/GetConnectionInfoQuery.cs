using System;
using Telemedicine.Common.Infrastructure.Patterns.Query;

namespace Telemedicine.Services.VideoConfIntegrService.WebAPI.Queries
{
    public record GetConnectionInfoQuery(Guid AppointmentId) : IQuery;
}
