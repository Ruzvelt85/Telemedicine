using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Services.WebClientBffService.API.HealthMeasurements.HealthMeasurementQueryService.Dto;
using Refit;

namespace Telemedicine.Services.WebClientBffService.API.HealthMeasurements.HealthMeasurementQueryService
{
    public interface IHealthMeasurementQueryService
    {
        [Get("/api/healthmeasurement")]
        Task<MeasurementListResponseDto> GetMeasurementList(GetMeasurementListRequestDto request, CancellationToken cancellationToken = default);
    }
}
