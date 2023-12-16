using FluentValidation;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate.Dto
{
    public class PulseRateMeasurementDtoValidator : AbstractValidator<PulseRateMeasurementDto>
    {
        public PulseRateMeasurementDtoValidator()
        {
            RuleFor(_ => _.PulseRate).NotEmpty();
        }
    }
}
