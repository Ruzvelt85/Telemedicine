using System;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto;
using Refit;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService
{
    public interface IAppointmentCommandService
    {
        [Put("/api/appointments/status")]
        Task UpdateStatus([Body(true)] UpdateAppointmentStatusRequestDto request, CancellationToken cancellationToken = default);

        [Post("/api/appointments")]
        Task<Guid> CreateAppointment([Body(true)] CreateAppointmentRequestDto request, CancellationToken cancellationToken = default);

        [Put("/api/appointments/{id}/reschedule")]
        Task<Guid> Reschedule(Guid id, [Body(true)] RescheduleAppointmentRequestDto request, CancellationToken cancellationToken = default);
    }
}
