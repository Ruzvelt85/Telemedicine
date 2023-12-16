using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.Patterns.Data;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Core.Repositories
{
    public interface IHealthCenterReadRepository : IReadRepository<HealthCenter>
    {
        Task<HealthCenter> SingleOrDefaultWithDeletedAsync(Expression<Func<HealthCenter, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
