namespace Telemedicine.Common.Infrastructure.DAL.MigrationsEfCoreCommon
{
    /// <summary>
    /// Setting to apply database seeding migrations
    /// </summary>
    public class DatabaseMigrationDataSettings
    {
        /// <summary>
        /// Is enable seeding migrations
        /// </summary>
        public bool IsEnabled { get; init; }

        /// <summary>
        /// Is delete all data from table (which will be overwritten) before applying seeding
        /// </summary>
        /// <remarks>usually not required</remarks>
        public bool IsDeleteDataFromTableBeforeSeeding { get; init; }

        /// <summary>
        /// Base path to JSON seeding files
        /// </summary>
        public string? JsonFilesBasePath { get; init; }
    }
}
