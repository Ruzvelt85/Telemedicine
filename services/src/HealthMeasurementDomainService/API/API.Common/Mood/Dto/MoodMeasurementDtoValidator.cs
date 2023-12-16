using FluentValidation;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto
{
    public class MoodMeasurementDtoValidator : AbstractValidator<MoodMeasurementDto>
    {
        public MoodMeasurementDtoValidator()
        {
            RuleFor(_ => _.Measure).NotEmpty().IsInEnum();
        }
    }
}