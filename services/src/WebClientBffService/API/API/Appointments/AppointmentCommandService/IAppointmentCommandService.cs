using System;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentCommandService.Dto;
using Refit;

namespace Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentCommandService
{
    public interface IAppointmentCommandService
    {
        [Post("/api/appointments")]
        Task<Guid> CreateAppointment([Body(true)] CreateAppointmentRequestDto request, CancellationToken cancellationToken = default);

        [Put("/api/appointments/cancel")]
        Task CancelAppointment([Body(true)] CancelAppointmentRequestDto request, CancellationToken cancellationToken = default);

        [Put("/api/appointments/{id}/reschedule")]
        Task<Guid> Reschedule(Guid id, [Body(true)] RescheduleAppointmentRequestDto request, CancellationToken cancellationToken = default);
    }
}
