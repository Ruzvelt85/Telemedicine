using FluentValidation;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Settings;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation.Dto
{
    public class SaturationMeasurementDtoValidator : AbstractValidator<SaturationMeasurementDto>
    {
        public SaturationMeasurementDtoValidator(ISaturationMeasurementSettingsBuilder saturationMeasurementSettingsBuilder)
        {
            var limit = saturationMeasurementSettingsBuilder.Build().MaxRawItemsValidationCountLimit;

            RuleFor(_ => _.SpO2).GreaterThan(0);
            RuleFor(_ => _.PulseRate).GreaterThanOrEqualTo(0).LessThanOrEqualTo(255);
            RuleFor(_ => _.Pi).GreaterThan(0);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            RuleFor(_ => _.RawMeasurements).Must(_ => _.Count <= limit).When(_ => _.RawMeasurements is not null);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
}
