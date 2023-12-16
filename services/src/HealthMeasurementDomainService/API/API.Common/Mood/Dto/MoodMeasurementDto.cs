namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto
{
    public record MoodMeasurementDto : IMeasurement
    {
        public MoodMeasureType Measure { get; init; }
    }
}