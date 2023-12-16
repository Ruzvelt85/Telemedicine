using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.EntityBase;

namespace Telemedicine.Common.Infrastructure.DomainEventInfrastructure
{
    /// <summary>
    /// Class that can publish a domain event in memory.
    /// </summary>
    public interface IDomainEventPublisher
    {
        Task PublishAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
