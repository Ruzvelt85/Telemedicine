using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.DomainEventInfrastructure;
using Telemedicine.Common.Infrastructure.EntityBase;
using Telemedicine.Common.Infrastructure.Patterns.Data;
using Serilog;

namespace Telemedicine.Common.Infrastructure.DAL.EfCoreDal
{
    /// <summary>
    /// The only endpoint for saving data in EF Core context.
    /// Work goes through EF Core context that underlies in implementation of repositories
    /// </summary>
    public sealed class UnitOfWork<TDbContext> : IUnitOfWork
        where TDbContext : EfCoreDbContext
    {
        private readonly TDbContext _context;
        private readonly ILogger _logger = Log.ForContext(typeof(UnitOfWork<TDbContext>));
        private readonly IDomainEventPublisher _domainEventPublisher;

        public UnitOfWork(TDbContext efCoreDbContext, IDomainEventPublisher domainEventPublisher)
        {
            _context = efCoreDbContext ?? throw new ArgumentNullException(nameof(efCoreDbContext));
            _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));

            _logger.Verbose("UnitOfWork created");
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            var events = _context.GetUnpublishedDomainEvents();

            await _context.SaveChangesAsync(cancellationToken);

            await DispatchEvents(events);
        }

        private async Task DispatchEvents(IEnumerable<DomainEvent> events)
        {
            foreach (var @event in events)
            {
                @event.IsPublished = true;
                await _domainEventPublisher.PublishAsync(@event);
            }
        }
    }
}
