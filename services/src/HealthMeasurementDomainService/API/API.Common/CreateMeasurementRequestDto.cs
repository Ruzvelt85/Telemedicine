using System;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common
{
    public record CreateMeasurementRequestDto<TMeasure> where TMeasure : IMeasurement, new()
    {
        public Guid PatientId { get; init; }

        public DateTime ClientDate { get; init; }

        public TMeasure Measure { get; init; } = new();
    }
}
