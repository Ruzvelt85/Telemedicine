using FluentValidation;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure.Dto
{
    public class BloodPressureMeasurementDtoValidator : AbstractValidator<BloodPressureMeasurementDto>
    {
        public BloodPressureMeasurementDtoValidator()
        {
            RuleFor(_ => _.Systolic).GreaterThan(0);
            RuleFor(_ => _.Diastolic).GreaterThan(0);
            RuleFor(_ => _.PulseRate).GreaterThan(0);
        }
    }
}