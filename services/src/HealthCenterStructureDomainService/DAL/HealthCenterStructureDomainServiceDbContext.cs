using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL.Configurations;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;

namespace Telemedicine.Services.HealthCenterStructureDomainService.DAL
{
    /// <summary>
    /// Entity framework context for Health Center Structure Service
    /// </summary>
    public class HealthCenterStructureDomainServiceDbContext : EfCoreDbContext
    {
#nullable disable
        public DbSet<User> Users { get; set; }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<InterdisciplinaryTeam> InterdisciplinaryTeams { get; set; }

        public virtual DbSet<HealthCenter> HealthCenters { get; set; }
#nullable restore

        public HealthCenterStructureDomainServiceDbContext(DbContextOptions options, IOptionsSnapshot<EfCoreDbContextSettings> settings, ILoggerFactory loggerFactory)
            : base(options, settings, loggerFactory)
        {
        }

        public HealthCenterStructureDomainServiceDbContext(DbContextOptions options, EfCoreDbContextSettings settings, ILoggerFactory loggerFactory)
            : base(options, settings, loggerFactory)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new DoctorConfiguration());
            modelBuilder.ApplyConfiguration(new PatientConfiguration());
            modelBuilder.ApplyConfiguration(new InterdisciplinaryTeamConfiguration());
            modelBuilder.ApplyConfiguration(new HealthCenterConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
