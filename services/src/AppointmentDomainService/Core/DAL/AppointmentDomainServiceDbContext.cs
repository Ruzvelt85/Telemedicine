using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.AppointmentDomainService.Core.DAL.Configurations;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Telemedicine.Services.AppointmentDomainService.Core.DAL
{
    /// <summary>
    /// Entity framework context for Appointment Service
    /// </summary>
    public class AppointmentDomainServiceDbContext : EfCoreDbContext
    {
#nullable disable
        public DbSet<Appointment> Appointments { get; set; }
#nullable restore

        public AppointmentDomainServiceDbContext(DbContextOptions options, IOptionsSnapshot<EfCoreDbContextSettings> settings, ILoggerFactory loggerFactory)
            : base(options, settings, loggerFactory)
        {
        }

        public AppointmentDomainServiceDbContext(DbContextOptions options, EfCoreDbContextSettings settings, ILoggerFactory loggerFactory)
            : base(options, settings, loggerFactory)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentAttendeeConfiguration());
        }
    }
}
