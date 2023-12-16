using Telemedicine.Common.Infrastructure.EntityBase;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;

namespace Telemedicine.Services.AppointmentDomainService.Core.Events
{
    public record AppointmentCancelledDomainEvent(Appointment Appointment) : DomainEvent;
}
