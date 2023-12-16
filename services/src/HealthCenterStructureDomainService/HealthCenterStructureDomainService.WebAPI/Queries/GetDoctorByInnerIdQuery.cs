using Telemedicine.Common.Infrastructure.Patterns.Query;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries
{
    public record GetDoctorByInnerIdQuery(string InnerId) : IQuery;
}
