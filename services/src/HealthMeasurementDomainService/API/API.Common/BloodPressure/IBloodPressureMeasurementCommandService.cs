using System;
using System.Threading;
using System.Threading.Tasks;
using Refit;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure.Dto;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure
{
    public interface IBloodPressureMeasurementCommandService
    {
        [Post("/api/bloodpressuremeasurements")]
        Task<Guid> CreateAsync([Body(true)] CreateMeasurementRequestDto<BloodPressureMeasurementDto> request, CancellationToken cancellationToken = default);
    }
}
