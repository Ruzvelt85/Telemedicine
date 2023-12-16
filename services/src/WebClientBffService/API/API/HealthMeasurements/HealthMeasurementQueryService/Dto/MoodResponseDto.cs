using System;
using Telemedicine.Services.WebClientBffService.API.Common;

namespace Telemedicine.Services.WebClientBffService.API.HealthMeasurements.HealthMeasurementQueryService.Dto
{
    public record MoodResponseDto : IHasClientDate
    {
        public Guid Id { get; init; }

        public MoodMeasureType Measure { get; init; }

        public DateTime ClientDate { get; init; }
    }
}
