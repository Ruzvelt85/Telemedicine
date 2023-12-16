namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure.Dto
{
    public record BloodPressureMeasurementDto : IMeasurement
    {
        public int Systolic { get; init; }

        public int Diastolic { get; init; }

        public int PulseRate { get; init; }
    }
}