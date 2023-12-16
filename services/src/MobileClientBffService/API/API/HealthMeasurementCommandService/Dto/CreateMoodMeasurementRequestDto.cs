using System;

namespace Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto
{
    public record CreateMoodMeasurementRequestDto
    {
        public MoodMeasureType Measure { get; init; }

        public DateTime ClientDate { get; init; }
    }
}
