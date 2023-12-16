using FluentValidation;

namespace Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto
{
    public class CreatePulseRateMeasurementRequestDtoValidator : AbstractValidator<CreatePulseRateMeasurementRequestDto>
    {
        public CreatePulseRateMeasurementRequestDtoValidator()
        {
            RuleFor(_ => _.PulseRate).GreaterThan(0);
            RuleFor(_ => _.ClientDate).NotEmpty();
        }
    }
}
