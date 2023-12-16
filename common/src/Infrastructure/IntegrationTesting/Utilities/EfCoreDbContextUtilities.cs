using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Microsoft.EntityFrameworkCore;

namespace Telemedicine.Common.Infrastructure.IntegrationTesting.Utilities
{
    /// <summary>
    /// Utility methods to work with <see cref="EfCoreDbContext"/>
    /// </summary>
    public static class EfCoreDbContextUtilities
    {
        /// <summary>
        /// Cleans/Truncates all tables (cascade) except tables related to entities with entityNames
        /// </summary>
        /// <param name="context">EF context</param>
        /// <param name="entityNames">Names of the entities to except from clean</param>
        public static async Task TruncateAllTablesExceptEntitiesAsync(EfCoreDbContext context, params string[] entityNames)
        {
            var tableNames = GetAllTableNamesExceptEntities(context, entityNames).ToArray();
            await TruncateTablesByNamesAsync(context, tableNames);
        }

        /// <summary>
        /// Cleans/Truncates the tables (cascade) by their names
        /// </summary>
        /// <param name="context">EF context</param>
        /// <param name="tableNames">Names of the tables</param>
        public static async Task TruncateTablesByNamesAsync(EfCoreDbContext context, params string[] tableNames)
        {
            var query = new StringBuilder();
            foreach (var tableName in tableNames)
            {
                query.AppendLine(GetTruncateTableExpression(tableName));
            }
            await ExecuteSqlRawAsync(context, query.ToString());
        }

        /// <summary>
        /// Get table names by context according to entityNames
        /// </summary>
        public static IEnumerable<string> GetAllTableNamesExceptEntities(EfCoreDbContext context, params string[] entityNames)
        {
            return context.Model.GetEntityTypes()
                .Where(x => !entityNames.Contains(x.ClrType.Name))
                .Select(x => x.GetTableName())
                .Where(x => !string.IsNullOrWhiteSpace(x)) //Filters null value, which could appear, if there are View in the database
                .Distinct();
        }

        /// <summary>
        /// Returns SQL query to clean/truncate the specified table (cascade)
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        private static string? GetTruncateTableExpression(string tableName)
#pragma warning disable EF1000 //The table name cannot be transferred via parameters, and if the query string is created by a concatenation or template - Warning EF1000
            => string.IsNullOrWhiteSpace(tableName) ? null : $"TRUNCATE \"{tableName}\" CASCADE;";
#pragma warning restore EF1000

        /// <summary>
        /// Execute raw sql query
        /// </summary>
        /// <param name="context">EfCoreDbContext</param>
        /// <param name="query">SQL query</param>
        public static async Task ExecuteSqlRawAsync(EfCoreDbContext context, string? query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return;
            }
            await context.Database.ExecuteSqlRawAsync(query);
        }
    }
}
