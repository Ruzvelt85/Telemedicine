using System;
using System.Threading;
using System.Threading.Tasks;
using Refit;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentQueryService.Dto;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentQueryService
{
    public interface IAppointmentQueryService
    {
        [Get("/api/appointments/{id}")]
        Task<AppointmentByIdResponseDto> GetAppointmentByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
