using System;
using Telemedicine.Common.Infrastructure.Patterns.Query;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries
{
    public record GetUserInfoQuery(Guid UserId) : IQuery;
}
