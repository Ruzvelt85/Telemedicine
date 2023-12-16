using System;
using Telemedicine.Services.WebClientBffService.API.Common;

namespace Telemedicine.Services.WebClientBffService.API.HealthMeasurements.HealthMeasurementQueryService.Dto
{
    public record PulseRateResponseDto : IHasClientDate
    {
        public Guid Id { get; init; }

        public int PulseRate { get; init; }

        public DateTime ClientDate { get; init; }
    }
}
