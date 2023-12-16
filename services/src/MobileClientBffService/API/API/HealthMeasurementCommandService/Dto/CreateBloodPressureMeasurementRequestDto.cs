using System;

namespace Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto
{
    public record CreateBloodPressureMeasurementRequestDto
    {
        public int Systolic { get; init; }

        public int Diastolic { get; init; }

        public int PulseRate { get; init; }

        public DateTime ClientDate { get; init; }
    }
}
