using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telemedicine.Common.Infrastructure.EntityBase;

namespace Telemedicine.Common.Infrastructure.DAL.EfCoreDal
{
    /// <summary>
    ///     <see cref="DbContext"/> with additional common logic and behavior.
    /// </summary>
    public abstract class EfCoreDbContext : DbContext
    {
        private readonly IOptionsSnapshot<EfCoreDbContextSettings>? _settingsSnapshot;
        private readonly EfCoreDbContextSettings? _settings;
        private readonly ILoggerFactory _loggerFactory;
        //private readonly ILogger _logger;

        /// <summary>
        /// Settings for Ef Core DbContext from configuration.
        /// In common case these settings are stored in _settingsSnapshot,
        /// but when EF Core context is being created through factory in Migrations projects -
        /// actual settings should be received from _settings
        /// </summary>
        protected EfCoreDbContextSettings DbContextSettings => _settingsSnapshot?.Value ?? _settings!;

        /// <inheritdoc/>
        protected EfCoreDbContext(DbContextOptions options, IOptionsSnapshot<EfCoreDbContextSettings> settings, ILoggerFactory loggerFactory) : base(options)
        {
            CheckParameter(options, nameof(options));
            CheckParameter(settings, nameof(settings));
            CheckParameter(loggerFactory, nameof(loggerFactory));

            _settingsSnapshot = settings;
            _loggerFactory = loggerFactory;
            //_logger = loggerFactory.CreateLogger(GetType());
        }

        /// <summary>
        /// This constructor should be used ONLY for creating EF context from DBContextFactory implemented in migrations projects 
        /// </summary>
        protected EfCoreDbContext(DbContextOptions options, EfCoreDbContextSettings settings, ILoggerFactory loggerFactory) : base(options)
        {
            CheckParameter(options, nameof(options));
            CheckParameter(settings, nameof(settings));
            CheckParameter(loggerFactory, nameof(loggerFactory));

            _settings = settings;
            _loggerFactory = loggerFactory;
            //_logger = loggerFactory.CreateLogger(GetType());
        }

        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(_loggerFactory); //For logging SQL-query

            if (DbContextSettings.IsEnabledSensitiveDataLogging)
            {
                optionsBuilder.EnableSensitiveDataLogging(); //For logging SQL-parameters
            }
        }

        /// <summary>
        /// Checks if parameter's value is null
        /// </summary>
        private void CheckParameter(object parameter, string parameterName)
        {
            if (parameter == null)
            {
                string message = $"EfCoreDbContext constructor: '{parameterName}' parameter is NULL.";
                Debug.Assert(false, message);
                throw new ArgumentNullException(message);
            }
        }

        /// <summary>
        /// Marks any "Removed" Entities as "Modified" and then sets the Db [IsDeleted] Flag to true
        /// </summary>
        public override int SaveChanges()
        {
            UpdateTechnicalFields();
            return base.SaveChanges();
        }

        /// <summary>
        /// Marks any "Removed" Entities as "Modified" and then sets the Db [IsDeleted] Flag to true
        /// </summary>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            UpdateTechnicalFields();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public IReadOnlyCollection<DomainEvent> GetUnpublishedDomainEvents()
        {
            var events = ChangeTracker.Entries<IHasDomainEvents>()
                .Select(x => x.Entity.DomainEvents)
                .SelectMany(x => x)
                .Where(domainEvent => !domainEvent.IsPublished)
                .ToArray();
            return events;
        }

        private void UpdateTechnicalFields()
        {
            ChangeTracker.DetectChanges();

            foreach (var item in ChangeTracker.Entries())
            {
                UpdateAuditFields(item);
                UpdateLogicalDeleteStatuses(item); //This must be executed last
            }
        }

        private void UpdateLogicalDeleteStatuses(EntityEntry item)
        {
            if (item.State == EntityState.Deleted && item.Entity is ILogicallyDeletable)
            {
                item.State = EntityState.Unchanged; // Set the entity to unchanged (if we mark the whole entity as Modified, every field gets sent to Db as an update)
                item.CurrentValues[nameof(ILogicallyDeletable.IsDeleted)] = true; // Only update the IsDeleted flag - only this will get sent to the Db
            }
        }

        private void UpdateAuditFields(EntityEntry item)
        {
            if (item.Entity is IAuditable)
            {
                var now = DateTime.UtcNow;
                switch (item.State)
                {
                    case EntityState.Modified:
                        item.CurrentValues[nameof(IAuditable.UpdatedOn)] = now;
                        break;
                    case EntityState.Added:
                        item.CurrentValues[nameof(IAuditable.CreatedOn)] = item.CurrentValues[nameof(IAuditable.UpdatedOn)] = now;
                        break;
                    case EntityState.Deleted:
                        item.CurrentValues[nameof(IAuditable.UpdatedOn)] = now;
                        break;
                }
            }
        }
    }
}
