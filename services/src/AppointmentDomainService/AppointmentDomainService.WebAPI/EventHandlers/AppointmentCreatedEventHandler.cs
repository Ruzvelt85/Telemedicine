using Telemedicine.Common.Infrastructure.DomainEventInfrastructure;
using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.CorrelationId;
using Telemedicine.Common.Infrastructure.Patterns.EventBus;
using Telemedicine.Services.AppointmentDomainService.API.IntegrationEvents;
using Telemedicine.Services.AppointmentDomainService.Core.Events;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.EventHandlers
{
    public class AppointmentCreatedEventHandler : BasePublisherEventHandler<AppointmentCreatedDomainEvent>
    {
        public AppointmentCreatedEventHandler(IEventBusPublisher eventBusPublisher, ICorrelationIdAccessor correlationIdAccessor)
            : base(eventBusPublisher, correlationIdAccessor)
        {
        }

        protected override IntegrationEvent GetIntegrationEvent(AppointmentCreatedDomainEvent domainEvent)
        {
            var integrationEvent = new AppointmentCreatedIntegrationEvent(
                domainEvent.Appointment.Id,
                domainEvent.Appointment.Duration,
                domainEvent.Appointment.StartDate,
                domainEvent.Appointment.Title,
                CorrelationIdAccessor.CorrelationId);
            return integrationEvent;
        }
    }
}
