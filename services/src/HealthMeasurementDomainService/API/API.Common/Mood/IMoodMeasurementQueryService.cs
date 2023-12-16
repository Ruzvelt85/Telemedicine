using System.Threading;
using System.Threading.Tasks;
using Refit;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood
{
    public interface IMoodMeasurementQueryService
    {
        [Get("/api/moodmeasurements")]
        Task<PagedListResponseDto<MeasurementResponseDto<MoodMeasurementDto>>> GetMoodList(GetMeasurementListRequestDto request, CancellationToken cancellationToken = default);
    }
}
