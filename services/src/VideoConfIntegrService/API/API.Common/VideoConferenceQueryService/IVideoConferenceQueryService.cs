using System;
using System.Threading;
using System.Threading.Tasks;
using Refit;
using Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceQueryService.Dto;

namespace Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceQueryService
{
    public interface IVideoConferenceQueryService
    {
        [Get("/api/conferences/{appointmentId}")]
        Task<ConnectionInfoResponseDto> GetConnectionInfoAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    }
}
