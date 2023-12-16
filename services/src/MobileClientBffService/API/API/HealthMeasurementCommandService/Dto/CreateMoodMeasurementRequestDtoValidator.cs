using FluentValidation;

namespace Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto
{
    public class CreateMoodMeasurementRequestDtoValidator : AbstractValidator<CreateMoodMeasurementRequestDto>
    {
        public CreateMoodMeasurementRequestDtoValidator()
        {
            RuleFor(_ => _.Measure).NotEmpty().IsInEnum();
            RuleFor(_ => _.ClientDate).NotEmpty();
        }
    }
}
