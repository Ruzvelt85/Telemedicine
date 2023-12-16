using Microsoft.EntityFrameworkCore;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Common.Infrastructure.DAL.MigrationsEfCoreCommon;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;

namespace Telemedicine.Services.HealthCenterStructureDomainService.MigrationEfCore
{
    /// <summary>
    /// It's implementation of the EF interface which generate DB Context when developer uses CLI
    /// </summary>
    [PublicAPI]
    public class HealthCenterStructureDomainServiceDbContextFactory : BaseDbContextFactory<HealthCenterStructureDomainServiceDbContext>
    {
        /// <summary>
        /// Create concrete <see cref="DbContext"/>
        /// </summary>
        protected override HealthCenterStructureDomainServiceDbContext CreateDbContext(DbContextOptions<HealthCenterStructureDomainServiceDbContext> options, EfCoreDbContextSettings settings, ILoggerFactory loggerFactory)
        {
            return new HealthCenterStructureDomainServiceDbContext(options, settings, loggerFactory);
        }
    }
}
