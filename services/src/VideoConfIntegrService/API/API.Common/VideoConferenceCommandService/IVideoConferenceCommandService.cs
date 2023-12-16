using System;
using System.Threading;
using System.Threading.Tasks;
using Refit;
using Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceCommandService.Dto;

namespace Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceCommandService
{
    public interface IVideoConferenceCommandService
    {
        [Post("/api/conferences")]
        Task<Guid> CreateAsync([Body(true)] CreateConferenceRequestDto request, CancellationToken cancellationToken = default);
    }
}
