using System;
using System.Threading;
using System.Threading.Tasks;
using Refit;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood
{
    public interface IMoodMeasurementCommandService
    {
        [Post("/api/moodmeasurements")]
        Task<Guid> CreateAsync([Body(true)] CreateMeasurementRequestDto<MoodMeasurementDto> request, CancellationToken cancellationToken = default);
    }
}
