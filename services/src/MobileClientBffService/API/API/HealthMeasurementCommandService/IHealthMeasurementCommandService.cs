using System;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto;
using Refit;

namespace Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService
{
    public interface IHealthMeasurementCommandService
    {
        [Post("/api/healthmeasurements/moods")]
        Task<Guid> CreateMoodMeasurementAsync([Body(true)] CreateMoodMeasurementRequestDto request, CancellationToken cancellationToken = default);

        [Post("/api/healthmeasurements/saturations")]
        Task<Guid> CreateSaturationMeasurementAsync([Body(true)] CreateSaturationMeasurementRequestDto request, CancellationToken cancellationToken = default);

        [Post("/api/healthmeasurements/bloodpressures")]
        Task<Guid> CreateBloodPressureMeasurementAsync([Body(true)] CreateBloodPressureMeasurementRequestDto request, CancellationToken cancellationToken = default);

        [Post("/api/healthmeasurements/pulserate")]
        Task<Guid> CreatePulseRateMeasurementAsync([Body(true)] CreatePulseRateMeasurementRequestDto request, CancellationToken cancellationToken = default);
    }
}
