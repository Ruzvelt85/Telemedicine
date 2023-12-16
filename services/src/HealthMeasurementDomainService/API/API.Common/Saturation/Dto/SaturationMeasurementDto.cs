using System.Collections.Generic;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation.Dto
{
    public record SaturationMeasurementDto : IMeasurement
    {
        public int SpO2 { get; init; }

        public int PulseRate { get; init; }

        public decimal Pi { get; init; }

        public IReadOnlyCollection<RawSaturationMeasurementItemDto>? RawMeasurements { get; init; }
    }
}