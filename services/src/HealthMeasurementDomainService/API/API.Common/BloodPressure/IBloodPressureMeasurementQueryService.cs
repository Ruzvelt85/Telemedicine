using System.Threading;
using System.Threading.Tasks;
using Refit;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure.Dto;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure
{
    public interface IBloodPressureMeasurementQueryService
    {
        [Get("/api/bloodpressuremeasurements")]
        Task<PagedListResponseDto<MeasurementResponseDto<BloodPressureMeasurementDto>>> GetBloodPressureList(GetMeasurementListRequestDto request, CancellationToken cancellationToken = default);
    }
}
