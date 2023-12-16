using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup.HasDomainEvents;
using Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup.Simple;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup
{
    /// <summary>
    /// Test Entity framework context 
    /// </summary>
    public class TestDbContext : EfCoreDbContext
    {
        public DbSet<TestEntity> Entities { get; set; } = default!;

        public DbSet<TestSimpleEntity> SimpleEntities { get; set; } = default!;

        public DbSet<TestLogicallyDeletedAuditableEntity> LogicallyDeletedAuditableEntities { get; set; } = default!;


        public TestDbContext(DbContextOptions options, IOptionsSnapshot<EfCoreDbContextSettings> settings, ILoggerFactory loggerFactory) : base(options, settings, loggerFactory)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new TestDbConfiguration());
            modelBuilder.ApplyConfiguration(new TestHasDomainEventsDbConfiguration());
        }
    }
}
