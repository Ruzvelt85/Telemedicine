using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.DAL.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Telemedicine.Services.HealthMeasurementDomainService.DAL
{
    /// <summary>
    /// Entity framework context for Health Measurement Service
    /// </summary>
    public class HealthMeasurementDomainServiceDbContext : EfCoreDbContext
    {
#nullable disable
        public DbSet<MoodMeasurement> MoodMeasurements { get; set; }

        public DbSet<SaturationMeasurement> SaturationMeasurements { get; set; }

        public DbSet<RawSaturationData> RawSaturationData { get; set; }

        public DbSet<PulseRateMeasurement> PulseRateMeasurements { get; set; }

        public DbSet<BloodPressureMeasurement> BloodPressureMeasurements { get; set; }
#nullable restore

        public HealthMeasurementDomainServiceDbContext(DbContextOptions options, IOptionsSnapshot<EfCoreDbContextSettings> settings, ILoggerFactory loggerFactory)
            : base(options, settings, loggerFactory)
        {
        }

        public HealthMeasurementDomainServiceDbContext(DbContextOptions options, EfCoreDbContextSettings settings, ILoggerFactory loggerFactory)
            : base(options, settings, loggerFactory)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new MoodMeasurementConfiguration());
            modelBuilder.ApplyConfiguration(new PulseRateMeasurementConfiguration());
            modelBuilder.ApplyConfiguration(new BloodPressureMeasurementConfiguration());
            modelBuilder.ApplyConfiguration(new SaturationMeasurementConfiguration());
            modelBuilder.ApplyConfiguration(new RawSaturationDataConfiguration());
        }
    }
}
