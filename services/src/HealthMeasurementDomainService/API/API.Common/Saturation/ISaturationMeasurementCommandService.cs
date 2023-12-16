using System;
using System.Threading;
using System.Threading.Tasks;
using Refit;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation.Dto;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation
{
    public interface ISaturationMeasurementCommandService
    {
        [Post("/api/saturationmeasurements")]
        Task<Guid> CreateAsync([Body(true)] CreateMeasurementRequestDto<SaturationMeasurementDto> request, CancellationToken cancellationToken = default);
    }
}
