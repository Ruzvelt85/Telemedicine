using JetBrains.Annotations;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Common.Infrastructure.DAL.MigrationsEfCoreCommon;
using Telemedicine.Services.HealthMeasurementDomainService.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Telemedicine.Services.HealthMeasurementDomainService.MigrationEfCore
{
    /// <summary>
    /// It's implementation of the EF interface which generate DB Context when developer uses CLI
    /// </summary>
    [PublicAPI]
    public class HealthMeasurementDomainServiceDbContextFactory : BaseDbContextFactory<HealthMeasurementDomainServiceDbContext>
    {
        /// <summary>
        /// Create concrete <see cref="DbContext"/>
        /// </summary>
        protected override HealthMeasurementDomainServiceDbContext CreateDbContext(DbContextOptions<HealthMeasurementDomainServiceDbContext> options, EfCoreDbContextSettings settings, ILoggerFactory loggerFactory)
        {
            return new HealthMeasurementDomainServiceDbContext(options, settings, loggerFactory);
        }
    }
}
