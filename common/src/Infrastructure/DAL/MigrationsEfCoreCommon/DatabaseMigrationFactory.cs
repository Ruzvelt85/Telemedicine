using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Serilog;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;

namespace Telemedicine.Common.Infrastructure.DAL.MigrationsEfCoreCommon
{
    public class DatabaseMigrationFactory
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(DatabaseMigrationFactory));

        public DatabaseMigration<TDbContext> BuildMigration<TDbContext>(IConfiguration configuration, TDbContext context)
                where TDbContext : EfCoreDbContext
        {
            DatabaseMigrationSchemaSettings? schemaSettings = configuration.GetSettings<DatabaseMigrationSchemaSettings>();
            ThrowExceptionIfNull(schemaSettings);
            DatabaseMigrationDataSettings? dataSettings = configuration.GetSettings<DatabaseMigrationDataSettings>();
            ThrowExceptionIfNull(dataSettings);
#pragma warning disable CS8604
            return new DatabaseMigration<TDbContext>(Assembly.GetExecutingAssembly(), context, schemaSettings, dataSettings);
#pragma warning restore CS8604
        }

        private static void ThrowExceptionIfNull<T>(T? settings)
        {
            if (settings is not null)
            { return; }

            var settingsName = typeof(T).Name;
            _logger.Error("Error occurred while getting settings for {SettingsName}, because the specified settings couldn't be found", settingsName);
            Debug.Assert(false, $"Error while getting settings for {settingsName}, because the specified settings couldn't be found");
            throw new ConfigurationMissingException(settingsName);
        }
    }
}
