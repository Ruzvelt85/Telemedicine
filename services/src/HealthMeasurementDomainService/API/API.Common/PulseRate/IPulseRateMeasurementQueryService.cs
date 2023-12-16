using Refit;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate.Dto;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate
{
    public interface IPulseRateMeasurementQueryService
    {
        [Get("/api/pulseratemeasurements")]
        Task<PagedListResponseDto<MeasurementResponseDto<PulseRateMeasurementDto>>> GetPulseRateList(GetMeasurementListRequestDto request, CancellationToken cancellationToken = default);
    }
}
