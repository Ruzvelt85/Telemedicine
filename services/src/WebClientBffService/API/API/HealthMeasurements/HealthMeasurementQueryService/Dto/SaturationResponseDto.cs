using System;
using Telemedicine.Services.WebClientBffService.API.Common;

namespace Telemedicine.Services.WebClientBffService.API.HealthMeasurements.HealthMeasurementQueryService.Dto
{
    public record SaturationResponseDto : IHasClientDate
    {
        public Guid Id { get; init; }

        public int SpO2 { get; init; }

        public int PulseRate { get; init; }

        public DateTime ClientDate { get; init; }
    }
}
