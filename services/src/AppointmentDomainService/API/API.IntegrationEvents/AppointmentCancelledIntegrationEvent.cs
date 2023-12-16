using System;
using Telemedicine.Common.Infrastructure.Patterns.EventBus;

namespace Telemedicine.Services.AppointmentDomainService.API.IntegrationEvents
{
    public record AppointmentCancelledIntegrationEvent(Guid AppointmentId, string? CancelReason, string? CorrelationId)
        : IntegrationEvent(CorrelationId);
}
