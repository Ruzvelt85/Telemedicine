using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.AppointmentDomainService.Core.DAL.Configurations;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.QueryHandlers.Setup
{
    /// <summary>
    /// Test Entity framework context
    /// </summary>
    public class AppointmentTestDbContext : EfCoreDbContext
    {
#nullable disable // DbSets are filled by EntityFramework
        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<AppointmentAttendee> AppointmentAttendees { get; set; }

#nullable restore

        public AppointmentTestDbContext(DbContextOptions options, IOptionsSnapshot<EfCoreDbContextSettings> settings, ILoggerFactory loggerFactory) : base(options, settings, loggerFactory)
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
