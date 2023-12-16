using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Microsoft.EntityFrameworkCore;

namespace Telemedicine.Common.Infrastructure.DAL.MigrationsEfCoreCommon
{
    /// <summary>
    /// SQL utilities methods which operate raw SQL queries
    /// </summary>
    public static class RawSqlUtilities
    {
        /// <summary>
        /// SQL query to remove EF Core migration table
        /// </summary>
        private const string DropMigrationTableExpression =
           "DROP TABLE IF EXISTS public.\"__EFMigrationsHistory\";";


        /// <summary>
        /// Execute raw sql query
        /// </summary>
        /// <param name="context">EfCoreDbContext</param>
        /// <param name="query">SQL query</param>
        public static async Task ExecuteSqlRawAsync(EfCoreDbContext context, string? query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return;
            }
            await context.Database.ExecuteSqlRawAsync(query);
        }

        /// <summary>
        /// Removes EF Core migration table
        /// </summary>
        public static async Task DropMigrationTableAsync<TDbContext>(TDbContext context)
            where TDbContext : EfCoreDbContext
            => await ExecuteSqlRawAsync(context, DropMigrationTableExpression);

        /// <summary>
        /// Cleans/Truncates the table (cascade)
        /// </summary>
        /// <param name="context">EF context</param>
        /// <param name="tableName">Name of the table</param>
        public static async Task TruncateTableAsync(EfCoreDbContext context, string tableName)
        {
            var query = GetTruncateTableExpression(tableName);
            await ExecuteSqlRawAsync(context, query);
        }

        /// <summary>
        /// Removes/Drops the table, if exists (cascade)
        /// </summary>
        /// <param name="context">EF context</param>
        /// <param name="tableName">Name of the table</param>
        public static async Task DropTableIfExistsAsync<TDbContext>(TDbContext context, string tableName)
            where TDbContext : EfCoreDbContext
        {
            var query = GetDropTableIfExistsExpression(tableName);
            await ExecuteSqlRawAsync(context, query);
        }

        /// <summary>
        /// Removes/Drops  collation, if exists
        /// </summary>
        /// <param name="context">EF context</param>
        /// <param name="collationName">Name of the collation</param>
        public static async Task DropCollationAsync<TDbContext>(TDbContext context, string collationName)
            where TDbContext : EfCoreDbContext
        {
            var dropCollationQuery = GetDropCollationIfExistsExpression(collationName);
            await ExecuteSqlRawAsync(context, dropCollationQuery);
        }

        /// <summary>
        /// Removes (Drop) extension, if exists
        /// </summary>
        /// <param name="context">EF context</param>
        /// <param name="extensionName">Name of the extension</param>
        public static async Task DropExtensionAsync<TDbContext>(TDbContext context, string extensionName)
            where TDbContext : EfCoreDbContext
        {
            var dropExtensionQuery = GetDropExtensionIfExistsExpression(extensionName);
            await ExecuteSqlRawAsync(context, dropExtensionQuery);
        }

        /// <summary>
        /// Returns SQL query to clean/truncate the specified table (cascade)
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        private static string? GetTruncateTableExpression(string tableName)
#pragma warning disable EF1000 //The table name cannot be transferred via parameters, and if the query string is created by a concatenation or template - Warning EF1000
            => string.IsNullOrEmpty(tableName) ? null : $"TRUNCATE \"{tableName}\" CASCADE;";
#pragma warning restore EF1000

        /// <summary>
        /// Returns SQL query to remove (drop) specified table (cascade)
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        private static string? GetDropTableIfExistsExpression(string tableName)
#pragma warning disable EF1000 //The table name cannot be transferred via parameters, and if the query string is created by a concatenation or template - Warning EF1000
            => string.IsNullOrEmpty(tableName) ? null : $"DROP TABLE IF EXISTS public.\"{tableName}\" CASCADE;";
#pragma warning restore EF1000

        /// <summary>
        /// Returns SQL query to remove (drop) specified collation
        /// </summary>
        /// <param name="collationName">Name of the collation</param>
        private static string? GetDropCollationIfExistsExpression(string collationName)
            => string.IsNullOrEmpty(collationName) ? null : $"DROP COLLATION IF EXISTS \"{collationName}\"";

        /// <summary>
        /// Returns sql query to remove (drop) extension
        /// </summary>
        /// <param name="extensionName">Name of the extension</param>
        private static string? GetDropExtensionIfExistsExpression(string extensionName)
            => string.IsNullOrEmpty(extensionName) ? null : $"DROP EXTENSION IF EXISTS \"{extensionName}\"";
    }
}
