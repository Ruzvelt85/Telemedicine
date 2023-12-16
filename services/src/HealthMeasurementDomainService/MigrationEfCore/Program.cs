using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.DAL.MigrationsEfCoreCommon;
using Microsoft.Extensions.Configuration;

namespace Telemedicine.Services.HealthMeasurementDomainService.MigrationEfCore
{
    class Program
    {
        static async Task Main()
        {
            IConfiguration configuration = ConfigurationUtilities.GetConfigurationBuilder().Build();
            await using var dbContext = new HealthMeasurementDomainServiceDbContextFactory().CreateDbContext(null);

            await new DatabaseMigrationFactory().BuildMigration(configuration, dbContext).ApplyAllAsync();
        }
    }
}
