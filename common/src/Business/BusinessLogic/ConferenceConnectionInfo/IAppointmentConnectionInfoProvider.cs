using System;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Dto;

namespace Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo
{
    public interface IAppointmentConnectionInfoProvider
    {
        Task<AppointmentInfoDto> GetAppointmentInfoAsync(Guid appointmentId, CancellationToken cancellationToken = default);

        Task<AppointmentConnectionInfoResponseDto> GetConnectionInfoAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    }
}
