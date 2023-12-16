using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Common.Infrastructure.DAL.MigrationsEfCoreCommon;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;

namespace Telemedicine.Services.AppointmentDomainService.MigrationEfCore
{

    /// <summary>
    /// It's implementation of the EF interface which generate DB Context when developer uses CLI
    /// </summary>
    [PublicAPI]
    public class AppointmentDomainServiceDbContextFactory : BaseDbContextFactory<AppointmentDomainServiceDbContext>
    {
        /// <summary>
        /// Create concrete <see cref="DbContext"/>
        /// </summary>
        protected override AppointmentDomainServiceDbContext CreateDbContext(DbContextOptions<AppointmentDomainServiceDbContext> options, EfCoreDbContextSettings settings, ILoggerFactory loggerFactory)
        {
            return new AppointmentDomainServiceDbContext(options, settings, loggerFactory);
        }
    }
}
