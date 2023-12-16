using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.VideoConfIntegrService.Core.Entities;
using Telemedicine.Services.VideoConfIntegrService.DAL.Configurations;

namespace Telemedicine.Services.VideoConfIntegrService.DAL
{
    /// <summary>
    /// Entity framework context for Video Conference Integration Service
    /// </summary>
    public class VideoConferenceIntegrationServiceDbContext : EfCoreDbContext
    {
#nullable disable
        public DbSet<Conference> Conferences { get; set; }
#nullable restore

        public VideoConferenceIntegrationServiceDbContext(DbContextOptions options, IOptionsSnapshot<EfCoreDbContextSettings> settings, ILoggerFactory loggerFactory)
            : base(options, settings, loggerFactory)
        {
        }

        public VideoConferenceIntegrationServiceDbContext(DbContextOptions options, EfCoreDbContextSettings settings, ILoggerFactory loggerFactory)
            : base(options, settings, loggerFactory)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ConferenceConfiguration());
        }
    }
}
