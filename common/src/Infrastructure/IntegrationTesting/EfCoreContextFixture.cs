using System;
using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Common.Infrastructure.IntegrationTesting.Utilities;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace Telemedicine.Common.Infrastructure.IntegrationTesting
{
    public class EfCoreContextFixture<TDbContext> : IDisposable where TDbContext : EfCoreDbContext
    {
        private readonly IConfigurationRoot _configuration;

        public EfCoreContextFixture()
        {
            _configuration = new ConfigurationBuilder().SetupConfigurationBuilderForTests().Build();
            DbContext = CreateDbContext();
        }

        public TDbContext DbContext { get; }

        public async Task CleanAllTablesExcept(params string[] entityNames)
        {
            await EfCoreDbContextUtilities.TruncateAllTablesExceptEntitiesAsync(DbContext, entityNames);
        }

        private TDbContext CreateDbContext()
        {
            var connectionString = _configuration.GetConnectionString("Main");
            var options = DatabaseConfigureUtility.GetOptions(new DbContextOptionsBuilder<TDbContext>(), connectionString);
            var settings = _configuration.GetSettings<EfCoreDbContextSettings>() ?? EfCoreDbContextSettings.Default;
            var loggerFactory = new NullLoggerFactory();
            var efCoreContext = Activator.CreateInstance(typeof(TDbContext), options.Options, settings, loggerFactory) as TDbContext;

            if (efCoreContext == null)
            {
                throw new Exception("Error occurred while creating EF Context for integration tests (EfCoreContextFixture): check the settings!");
            }

            return efCoreContext;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            DbContext.Dispose();
        }
    }
}
