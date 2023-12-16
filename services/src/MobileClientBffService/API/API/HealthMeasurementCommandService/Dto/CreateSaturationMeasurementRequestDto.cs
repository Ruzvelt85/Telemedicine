using System;
using System.Collections.Generic;

namespace Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto
{
    public record CreateSaturationMeasurementRequestDto
    {
        public int SpO2 { get; init; }

        public int PulseRate { get; init; }

        public decimal Pi { get; init; }

        public IReadOnlyCollection<RawSaturationItemRequestDto>? RawMeasurements { get; init; }

        public DateTime ClientDate { get; init; }
    }
}
