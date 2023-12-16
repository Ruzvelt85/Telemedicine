using System;
using Telemedicine.Common.Infrastructure.Patterns.EventBus;

namespace Telemedicine.Services.AppointmentDomainService.API.IntegrationEvents
{
    public record AppointmentCreatedIntegrationEvent(Guid AppointmentId,
            TimeSpan AppointmentDuration,
            DateTime AppointmentStartDate,
            string AppointmentTitle,
            string? CorrelationId)
        : IntegrationEvent(CorrelationId);
}
