using System;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation.Dto
{
    public record RawSaturationMeasurementItemDto
    {
        public int Order { get; init; }

        public int SpO2 { get; init; }

        public int PulseRate { get; init; }

        public decimal Pi { get; init; }

        public DateTime ClientDate { get; init; }
    }
}
