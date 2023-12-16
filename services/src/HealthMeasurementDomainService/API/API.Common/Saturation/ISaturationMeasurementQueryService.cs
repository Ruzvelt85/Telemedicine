using Refit;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation.Dto;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation
{
    public interface ISaturationMeasurementQueryService
    {
        [Get("/api/saturationmeasurements")]
        Task<PagedListResponseDto<MeasurementResponseDto<SaturationMeasurementDto>>> GetSaturationList(GetMeasurementListRequestDto request, CancellationToken cancellationToken = default);
    }
}