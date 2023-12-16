using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.DAL.MigrationsEfCoreCommon;

namespace Telemedicine.Services.VideoConfIntegrService.MigrationEfCore
{
    class Program
    {
        static async Task Main()
        {
            IConfiguration configuration = ConfigurationUtilities.GetConfigurationBuilder().Build();
            await using var dbContext = new VideoConferenceIntegrationServiceDbContextFactory().CreateDbContext(null);

            await new DatabaseMigrationFactory().BuildMigration(configuration, dbContext).ApplyAllAsync();
        }
    }
}
