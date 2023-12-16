namespace Telemedicine.Common.Infrastructure.DAL.EfCoreDal
{
    /// <summary>
    /// Setting to configure logging SQL-queries
    /// </summary>
    public class EfCoreDbContextSettings
    {
        public static EfCoreDbContextSettings Default = new();

        /// <summary>
        /// Is enable logging sensitive data (parameters of SQL-queries)
        /// </summary>
        /// <remarks>usually not required</remarks>
        public bool IsEnabledSensitiveDataLogging { get; init; } = false;
    }
}
