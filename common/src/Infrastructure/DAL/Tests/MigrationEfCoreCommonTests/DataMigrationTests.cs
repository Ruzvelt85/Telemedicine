using System;
using System.Linq;
using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Common.Infrastructure.DAL.MigrationsEfCoreCommon;
using Telemedicine.Common.Infrastructure.DAL.Tests.MigrationEfCoreCommonTests.TestSetup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.MigrationEfCoreCommonTests
{
    public class DataMigrationTests : IDisposable
    {
#nullable disable // Is initialized in TestSetUp
        private readonly TestDbContext _context;
#nullable restore

        public DataMigrationTests()
        {
            // For every unit test we have to specify different name of InMemory database
            // in order to prevent unstable results of tests due to only one service-provider
            var dbContextOptions =
                new DbContextOptionsBuilder<TestDbContext>()
                    .UseInMemoryDatabase($"DataMigrationTests-{Guid.NewGuid()}");

            _context = new TestDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());

            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task MigrateDataTest()
        {
            var dataSettings = new DatabaseMigrationDataSettings
            {
                IsEnabled = true,
                IsDeleteDataFromTableBeforeSeeding = false,
                JsonFilesBasePath = "./TestSeeds"
            };

            await ExecuteDataMigration(dataSettings);

            Assert.Equal(0, _context.Test5Entities.Count()); // no seed
            Assert.Equal(0, _context.Test3Entities.Count()); // blank seed file
            Assert.Equal(0, _context.Test4Entities.Count()); // seed with no elements
            Assert.Equal(1, _context.Test2Entities.Count()); // seed with 1 element
            Assert.Equal(3, _context.Test1Entities.Count()); // seed with 3 elements
        }

        [Fact]
        public async Task MigrateDataTestWithDisabledConfiguration()
        {
            var dataSettings = new DatabaseMigrationDataSettings
            {
                IsEnabled = false,
                IsDeleteDataFromTableBeforeSeeding = false,
                JsonFilesBasePath = "./TestSeeds"
            };

            await ExecuteDataMigration(dataSettings);

            Assert.Equal(0, _context.Test5Entities.Count());
            Assert.Equal(0, _context.Test3Entities.Count());
            Assert.Equal(0, _context.Test4Entities.Count());
            Assert.Equal(0, _context.Test2Entities.Count());
            Assert.Equal(0, _context.Test1Entities.Count());
        }

        private async Task ExecuteDataMigration(DatabaseMigrationDataSettings dataSettings)
        {
            var migration = new DatabaseMigration<TestDbContext>(
                typeof(DataMigrationTests).Assembly, _context,
                new DatabaseMigrationSchemaSettings(), dataSettings);

            await migration.ApplyAllAsync();
        }

        private Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());

            return mockOptions;
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
        }
    }
}
