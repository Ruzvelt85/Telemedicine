using System;
using System.Threading;
using System.Threading.Tasks;
using Refit;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Dto;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentById;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentList;

namespace Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService
{
    public interface IAppointmentQueryService
    {
        [Get("/api/appointments")]
        Task<AppointmentListResponseDto> GetAppointmentList(AppointmentListRequestDto request, CancellationToken cancellationToken = default);

        [Get("/api/appointments/{id}")]
        Task<AppointmentByIdResponseDto> GetAppointmentByIdAsync(Guid id, CancellationToken cancellationToken = default);

        [Get("/api/appointments/connectioninfo/{id}")]
        Task<AppointmentConnectionInfoResponseDto> GetAppointmentConnectionInfoAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
