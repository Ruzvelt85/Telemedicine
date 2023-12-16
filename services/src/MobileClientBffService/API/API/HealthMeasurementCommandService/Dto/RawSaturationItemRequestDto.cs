using System;

namespace Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto
{
    public record RawSaturationItemRequestDto
    {
        public int Order { get; init; }

        public int SpO2 { get; init; }

        public int PulseRate { get; init; }

        public decimal Pi { get; init; }

        public DateTime ClientDate { get; init; }
    }
}
