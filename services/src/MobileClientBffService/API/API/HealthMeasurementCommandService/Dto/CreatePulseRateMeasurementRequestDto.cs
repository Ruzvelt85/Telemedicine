using System;

namespace Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto
{
    public record CreatePulseRateMeasurementRequestDto
    {
        public int PulseRate { get; init; }

        public DateTime ClientDate { get; init; }
    }
}
