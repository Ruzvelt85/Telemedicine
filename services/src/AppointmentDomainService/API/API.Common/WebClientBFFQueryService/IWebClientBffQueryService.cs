using System;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;
using Refit;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService
{
    public interface IWebClientBffQueryService
    {
        [Get("/api/web/appointments")]
        Task<AppointmentListResponseDto> GetAppointmentList(AppointmentListRequestDto request, CancellationToken cancellationToken = default);

        [Get("/api/appointments/nearest")]
        Task<NearestAppointmentsResponseDto> GetNearestAppointmentsByAttendeeIdAsync(Guid attendeeId, CancellationToken cancellationToken = default);
    }
}
