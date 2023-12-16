using System;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.EntityBase;
using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.CorrelationId;
using Telemedicine.Common.Infrastructure.Patterns.EventBus;
using MediatR;
using Serilog;

namespace Telemedicine.Common.Infrastructure.DomainEventInfrastructure
{
    public abstract class BasePublisherEventHandler<TEvent>
        : INotificationHandler<DomainEventNotification<TEvent>> where TEvent : DomainEvent
    {
        private readonly ILogger _logger = Log.ForContext<BasePublisherEventHandler<TEvent>>();

        protected BasePublisherEventHandler(IEventBusPublisher eventBusPublisher, ICorrelationIdAccessor correlationIdAccessor)
        {
            EventBusPublisher = eventBusPublisher ?? throw new ArgumentNullException(nameof(eventBusPublisher));
            CorrelationIdAccessor = correlationIdAccessor ?? throw new ArgumentNullException(nameof(correlationIdAccessor));
        }

        protected IEventBusPublisher EventBusPublisher { get; init; }

        protected ICorrelationIdAccessor CorrelationIdAccessor { get; init; }

        public virtual async Task Handle(DomainEventNotification<TEvent> notification, CancellationToken cancellationToken)
        {
            try
            {
                var integrationEvent = GetIntegrationEvent(notification.DomainEvent);

                _logger.Information("Publishing integration event. Event - {Event}", integrationEvent.GetType().Name);
                await EventBusPublisher.TryPublishAsync(integrationEvent);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An exception occurred while publishing an integration event. Notification - {@Notification}", notification);
            }
        }

        protected abstract IntegrationEvent GetIntegrationEvent(TEvent domainEvent);
    }
}
