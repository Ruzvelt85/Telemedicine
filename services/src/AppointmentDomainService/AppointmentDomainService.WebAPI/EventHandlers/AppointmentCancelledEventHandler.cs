using Telemedicine.Common.Infrastructure.DomainEventInfrastructure;
using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.CorrelationId;
using Telemedicine.Common.Infrastructure.Patterns.EventBus;
using Telemedicine.Services.AppointmentDomainService.API.IntegrationEvents;
using Telemedicine.Services.AppointmentDomainService.Core.Events;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.EventHandlers
{
    public class AppointmentCancelledEventHandler : BasePublisherEventHandler<AppointmentCancelledDomainEvent>
    {
        public AppointmentCancelledEventHandler(IEventBusPublisher eventBusPublisher, ICorrelationIdAccessor correlationIdAccessor)
            : base(eventBusPublisher, correlationIdAccessor)
        {
        }

        protected override IntegrationEvent GetIntegrationEvent(AppointmentCancelledDomainEvent domainEvent)
        {
            var integrationEvent = new AppointmentCancelledIntegrationEvent(
                domainEvent.Appointment.Id,
                domainEvent.Appointment.CancelReason,
                CorrelationIdAccessor.CorrelationId);
            return integrationEvent;
        }
    }
}
