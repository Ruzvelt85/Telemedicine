namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate.Dto
{
    public class CreatePulseRateMeasurementRequestDtoValidator : CreateMeasurementRequestDtoValidator<PulseRateMeasurementDto>
    {
        public CreatePulseRateMeasurementRequestDtoValidator()
        {
            RuleFor(_ => _.Measure).SetValidator(new PulseRateMeasurementDtoValidator());
        }
    }
}
