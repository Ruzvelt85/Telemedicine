using System;
using System.Threading;
using System.Threading.Tasks;
using Refit;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Dto;

namespace Telemedicine.Services.MobileClientBffService.API.AppointmentQueryService
{
    public interface IAppointmentQueryService
    {
        [Get("/api/appointments/connectioninfo/{id}")]
        Task<AppointmentConnectionInfoResponseDto> GetAppointmentConnectionInfoAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
