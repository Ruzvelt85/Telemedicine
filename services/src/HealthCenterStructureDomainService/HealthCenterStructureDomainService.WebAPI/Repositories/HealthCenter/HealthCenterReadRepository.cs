using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.HealthCenter
{
    public class HealthCenterReadRepository : ReadRepository<Core.Entities.HealthCenter>, IHealthCenterReadRepository
    {
        /// <inheritdoc />
        public HealthCenterReadRepository(HealthCenterStructureDomainServiceDbContext context) : base(context)
        {
        }

        public Task<Core.Entities.HealthCenter> SingleOrDefaultWithDeletedAsync(Expression<Func<Core.Entities.HealthCenter, bool>> predicate, CancellationToken cancellationToken = default) =>
            DbSet.IgnoreQueryFilters().SingleOrDefaultAsync(predicate, cancellationToken);
    }
}
