namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate.Dto
{
    public record PulseRateMeasurementDto : IMeasurement
    {
        public int PulseRate { get; init; }
    }
}
