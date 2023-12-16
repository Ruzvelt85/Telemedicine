using System;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.EntityBase;
using MediatR;
using Serilog;

namespace Telemedicine.Common.Infrastructure.DomainEventInfrastructure
{
    public class DomainEventPublisher : IDomainEventPublisher
    {
        private readonly IPublisher _mediator;
        private readonly ILogger _logger = Log.ForContext<DomainEventPublisher>();

        public DomainEventPublisher(IPublisher mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task PublishAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _logger.Information("Publishing domain event. Event - {Event}", domainEvent.GetType().Name);

            try
            {
                var domainEventNotification = DomainEventNotificationFactory.Create(domainEvent);
                await _mediator.Publish(domainEventNotification, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An exception occurred while publishing an domain event. Event - {@Event}", domainEvent);
            }
        }
    }
}
