using System;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common
{
    public record MeasurementResponseDto<TMeasure> : IHasClientDate where TMeasure : IMeasurement, new()
    {
        public Guid Id { get; init; }

        public DateTime ClientDate { get; init; }

        public TMeasure Measure { get; init; } = new();
    }
}
