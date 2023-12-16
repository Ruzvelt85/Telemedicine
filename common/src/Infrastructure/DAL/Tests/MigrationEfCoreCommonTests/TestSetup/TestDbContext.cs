using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.MigrationEfCoreCommonTests.TestSetup
{
    /// <summary>
    /// Test Entity framework context
    /// </summary>
    public class TestDbContext : EfCoreDbContext
    {
#nullable disable // DbSets are filled by EntityFramework
        public DbSet<Test1Entity> Test1Entities { get; set; }

        public DbSet<Test2Entity> Test2Entities { get; set; }

        public DbSet<Test3Entity> Test3Entities { get; set; }

        public DbSet<Test4Entity> Test4Entities { get; set; }

        public DbSet<Relative> Test5Entities { get; set; }
#nullable restore

        public TestDbContext(DbContextOptions options, IOptionsSnapshot<EfCoreDbContextSettings> settings, ILoggerFactory loggerFactory) : base(options, settings, loggerFactory)
        {
        }
    }
}
