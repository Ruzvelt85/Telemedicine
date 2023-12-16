namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure.Dto
{
    public class CreateBloodPressureMeasurementRequestDtoValidator : CreateMeasurementRequestDtoValidator<BloodPressureMeasurementDto>
    {
        public CreateBloodPressureMeasurementRequestDtoValidator()
        {
            RuleFor(_ => _.Measure).SetValidator(new BloodPressureMeasurementDtoValidator());
        }
    }
}