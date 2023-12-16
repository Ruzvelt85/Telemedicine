using System;
using System.Threading;
using System.Threading.Tasks;
using Refit;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate.Dto;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate
{
    public interface IPulseRateMeasurementCommandService
    {
        [Post("/api/pulseratemeasurements")]
        Task<Guid> CreateAsync([Body(true)] CreateMeasurementRequestDto<PulseRateMeasurementDto> request, CancellationToken cancellationToken = default);
    }
}
