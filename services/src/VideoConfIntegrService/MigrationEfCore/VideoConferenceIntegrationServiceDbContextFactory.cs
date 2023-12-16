using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using JetBrains.Annotations;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Common.Infrastructure.DAL.MigrationsEfCoreCommon;
using Telemedicine.Services.VideoConfIntegrService.DAL;

namespace Telemedicine.Services.VideoConfIntegrService.MigrationEfCore
{
    /// <summary>
    /// It's implementation of the EF interface which generate DB Context when developer uses CLI
    /// </summary>
    [PublicAPI]
    public class VideoConferenceIntegrationServiceDbContextFactory : BaseDbContextFactory<VideoConferenceIntegrationServiceDbContext>
    {
        /// <summary>
        /// Create concrete <see cref="DbContext"/>
        /// </summary>
        protected override VideoConferenceIntegrationServiceDbContext CreateDbContext(DbContextOptions<VideoConferenceIntegrationServiceDbContext> options, EfCoreDbContextSettings settings, ILoggerFactory loggerFactory)
        {
            return new VideoConferenceIntegrationServiceDbContext(options, settings, loggerFactory);
        }
    }
}
