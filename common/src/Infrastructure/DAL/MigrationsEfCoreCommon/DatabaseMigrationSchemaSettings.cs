namespace Telemedicine.Common.Infrastructure.DAL.MigrationsEfCoreCommon
{
    /// <summary>
    /// Setting to apply database schema migrations
    /// </summary>
    public class DatabaseMigrationSchemaSettings
    {
        /// <summary>
        /// Is enable schema migrations
        /// </summary>
        public bool IsEnabled { get; init; }

        /// <summary>
        /// Is remove whole database before applying database schema
        /// </summary>
        /// <remarks>usually not required</remarks>
        public bool DropDatabaseBeforeMigrate { get; init; }

        /// <summary>
        /// Usually it's required on development environment and sometimes on testing environments
        /// </summary>
        public bool DropTablesBeforeMigrate { get; init; }
    }
}
