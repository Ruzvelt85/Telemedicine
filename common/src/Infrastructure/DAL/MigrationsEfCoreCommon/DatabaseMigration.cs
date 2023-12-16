using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Common.Infrastructure.DAL.MigrationsEfCoreCommon.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using Npgsql;
using Serilog;

namespace Telemedicine.Common.Infrastructure.DAL.MigrationsEfCoreCommon
{
    /// <summary>
    /// Utility class to apply migration and data fro tests
    /// </summary>
    [PublicAPI]
    public class DatabaseMigration<TDbContext> : IDisposable
        where TDbContext : EfCoreDbContext
    {
        private readonly Assembly _migrationsAssembly;
        private readonly DatabaseMigrationSchemaSettings _schemaSettings;
        private readonly DatabaseMigrationDataSettings _dataSettings;
        private readonly TDbContext _context;
        private readonly ILogger _logger = Log.ForContext<DatabaseMigration<TDbContext>>();

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="migrationsAssembly">Assembly which contains data for the migration</param>
        /// <param name="context">EF Core context</param>
        /// <param name="schemaSettings">Settings for database schema migration</param>
        /// <param name="dataSettings">Settings for database data migration</param>
        public DatabaseMigration(Assembly migrationsAssembly, TDbContext context,
            DatabaseMigrationSchemaSettings schemaSettings,
            DatabaseMigrationDataSettings dataSettings)
        {
            _schemaSettings = schemaSettings ?? throw new ArgumentNullException(nameof(schemaSettings));
            _dataSettings = dataSettings ?? throw new ArgumentNullException(nameof(dataSettings));
            _migrationsAssembly = migrationsAssembly ?? throw new ArgumentNullException(nameof(migrationsAssembly));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Apply migrations database schema and data
        /// </summary>
        public async Task ApplyAllAsync()
        {
            await ApplyDbSchemaMigrationsAsync();

            await ApplyDbDataMigrationsAsync();
        }

        /// <summary>
        /// Using the standard EF mechanism removes the database if necessary and saves to database all generated migrations
        /// </summary>
        public async Task ApplyDbSchemaMigrationsAsync()
        {
            if (!_schemaSettings.IsEnabled)
            {
                _logger.Information("DB scheme migration will not be applied due to configuration");
                return;
            }

            try
            {
                await DeleteOrClearDatabase();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error occurred while clearing database before applying scheme migration");
                throw;
            }

            try
            {
                await _context.Database.MigrateAsync();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error occurred while DB scheme migration");
                throw;
            }

            // We reload types after migrations, because of types get cached after first connection, so
            // after add new type (citext) they will not update
            // https://stackoverflow.com/questions/51086421
            if (_context.Database.GetDbConnection() is NpgsqlConnection npgsqlConnection)
            {
                await npgsqlConnection.OpenAsync();
                npgsqlConnection.ReloadTypes();
                await npgsqlConnection.CloseAsync();
            }

            _logger.Information("DB scheme migration is successfully done");
        }

        public async Task ApplyDbDataMigrationsAsync()
        {
            if (!_dataSettings.IsEnabled)
            {
                _logger.Information("DB seeding will not be applied due to configuration");
                return;
            }

            var allSeeds = GetAndValidateSeeds();
            if (allSeeds.Length == 0)
            { return; }

            IEntityType[] allEntityTypes = _context.Model.GetEntityTypes().Where(x => !x.IsOwned()).ToArray();

            // We need to save changes in database in several steps due to the presence of dependent entities with foreign keys
            foreach (var seeds in SeedsUtils.GetOrderedSeeds(allSeeds))
            { await SaveDataInDatabase(seeds, allEntityTypes); }

            _logger.Information("DB data migration is successfully done");
        }

        private async Task SaveDataInDatabase(IEnumerable<string> seeds, IEntityType[] allEntityTypes)
        {
            //TODO: Pokhunov JD-253 for types from dbContext find seed.
            foreach (var seed in seeds)
            {
                var seedFileName = Path.GetFileNameWithoutExtension(seed);
                // remove order prefix from seed filename
                var seedEntityName = seedFileName.Substring(seedFileName.IndexOf('_') + 1);
                var entityType = allEntityTypes.FirstOrDefault(x => seedEntityName == x.ClrType.Name);

                if (entityType == null)
                {
                    _logger.Warning("Entity type cannot be found for specified seed '{Seed}'", seed);
                    continue;
                }

                var destinationType = entityType.ClrType;
                var itemsForSeeding = await GetItemsForSeeding(seed, destinationType);

                if (itemsForSeeding == null || !itemsForSeeding.Any())
                {
                    _logger.Warning("There are no items for seeding: '{Seed}'", seed);
                    continue;
                }

                if (_dataSettings.IsDeleteDataFromTableBeforeSeeding)
                {
                    try
                    {
                        // Truncate table which will be overwritten
                        await RawSqlUtilities.TruncateTableAsync(_context, entityType.GetTableName());
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e, "Error occurred while truncating DB table {TableName}", entityType.GetTableName());
                        throw;
                    }
                }
                try
                {
                    await _context.AddRangeAsync(itemsForSeeding);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error occurred while adding data for seed: {Seed}", seed);
                    throw;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error occurred while DB data migration: saving data in database finished with error");
                throw;
            }
        }

        protected virtual async Task DeleteOrClearDatabase()
        {
            if (_schemaSettings.DropDatabaseBeforeMigrate)
            {
                await _context.Database.EnsureDeletedAsync();
            }
            else if (_schemaSettings.DropTablesBeforeMigrate) //Drops tables if database wasn't drop before
            {
                var uniqTableNames = GetAllTableNames();

                //Remove table with information about migrations
                await RawSqlUtilities.DropMigrationTableAsync(_context);

                foreach (var tableName in uniqTableNames)
                {
                    //Remove table if exists
                    await RawSqlUtilities.DropTableIfExistsAsync(_context, tableName);
                }
                //Remove collation if exists
                await RawSqlUtilities.DropCollationAsync(_context, "case_insensitive");

                //Remove extension "citext" if exists
                await RawSqlUtilities.DropExtensionAsync(_context, "citext");
            }
        }

        /// <summary>
        /// Gets all table names in context
        /// </summary>
        private IEnumerable<string> GetAllTableNames()
        {
            return _context.Model.GetEntityTypes()
                .Select(x => x.GetTableName())
                .Where(x => !string.IsNullOrEmpty(x)) //Filters null value, which could appear, if there are View in the database
                .Distinct();
        }

        /// <summary>
        /// Gets and validates files for seeding
        /// </summary>
        private string[] GetAndValidateSeeds()
        {
            string[] seeds;

            if (string.IsNullOrEmpty(_dataSettings.JsonFilesBasePath))
            {
                _logger.Information("DB seeding will not be applied due to configuration");
                return new string[] { };
            }

            try
            {
                seeds = Directory.GetFiles(_dataSettings.JsonFilesBasePath);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error occurred while DB data migration: the specified folder '{JsonFilesBasePath}' is not correct", _dataSettings.JsonFilesBasePath);
                return new string[] { };
            }

            if (seeds.Length == 0 || !seeds.Any(x => x.Contains(".json")))
            {
                _logger.Error("Error occurred while DB data migration: JSON-files are not found in the specified folder {JsonFilesBasePath}", _dataSettings.JsonFilesBasePath);
                return new string[] { };
            }

            return seeds;
        }

        /// <summary>
        /// Gets collection of items for seeding in database
        /// </summary>
        private async Task<List<object>?> GetItemsForSeeding(string fileName, Type? destinationType)
        {
            if (string.IsNullOrEmpty(fileName) || destinationType == null)
                return null;

            try
            {
                using (var stream = new StreamReader(fileName))
                {
                    var text = await stream.ReadToEndAsync();
                    var itemsFromJson = JsonConvert.DeserializeObject(text,
                        typeof(List<>).MakeGenericType(destinationType));

                    if (itemsFromJson == null)
                    {
                        _logger.Error("Items cannot be deserialized from specified JSON-file '{Seed}", fileName);
                        return null;
                    }

                    return ((IEnumerable<object>)itemsFromJson).ToList();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error occurred while DB data migration: information cannot be deserialized from specified JSON-file '{Seed}", fileName);
                throw;
            }
        }

        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }

        #endregion
    }
}
