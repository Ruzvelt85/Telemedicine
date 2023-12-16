using FluentValidation;

namespace Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto
{
    public class CreateBloodPressureMeasurementRequestDtoValidator : AbstractValidator<CreateBloodPressureMeasurementRequestDto>
    {
        public CreateBloodPressureMeasurementRequestDtoValidator()
        {
            RuleFor(_ => _.Systolic).GreaterThan(0);
            RuleFor(_ => _.Diastolic).GreaterThan(0);
            RuleFor(_ => _.PulseRate).GreaterThan(0);
            RuleFor(_ => _.ClientDate).NotEmpty();
        }
    }
}
