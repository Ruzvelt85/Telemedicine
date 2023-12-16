using FluentValidation;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common
{
    public abstract class CreateMeasurementRequestDtoValidator<TMeasure> : AbstractValidator<CreateMeasurementRequestDto<TMeasure>>
        where TMeasure : IMeasurement, new()
    {
        protected CreateMeasurementRequestDtoValidator()
        {
            RuleFor(_ => _.PatientId).NotEmpty();
            RuleFor(_ => _.ClientDate).NotEmpty();
        }
    }
}
