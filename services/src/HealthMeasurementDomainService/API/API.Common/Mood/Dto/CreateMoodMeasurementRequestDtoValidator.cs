namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto
{
    public class CreateMoodMeasurementRequestDtoValidator : CreateMeasurementRequestDtoValidator<MoodMeasurementDto>
    {
        public CreateMoodMeasurementRequestDtoValidator()
        {
            RuleFor(_ => _.Measure).SetValidator(new MoodMeasurementDtoValidator());
        }
    }
}
