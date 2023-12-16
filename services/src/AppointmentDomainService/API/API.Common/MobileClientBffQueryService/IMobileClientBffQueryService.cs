using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto;
using Refit;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService
{
    public interface IMobileClientBffQueryService
    {
        [Get("/api/mobile/appointments")]
        Task<AppointmentListResponseDto> GetAppointmentList(AppointmentListRequestDto request, CancellationToken cancellationToken = default);

        [Get("/api/mobile/appointments/changed")]
        Task<AppointmentListResponseDto> GetChangedAppointmentList(ChangedAppointmentListRequestDto request, CancellationToken cancellationToken = default);
    }
}
