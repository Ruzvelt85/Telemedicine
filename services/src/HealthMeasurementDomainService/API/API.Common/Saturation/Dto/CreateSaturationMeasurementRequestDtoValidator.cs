using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Settings;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation.Dto
{
    public class CreateSaturationMeasurementRequestDtoValidator : CreateMeasurementRequestDtoValidator<SaturationMeasurementDto>
    {
        public CreateSaturationMeasurementRequestDtoValidator(ISaturationMeasurementSettingsBuilder saturationMeasurementSettingsBuilder)
        {
            RuleFor(_ => _.Measure).SetValidator(new SaturationMeasurementDtoValidator(saturationMeasurementSettingsBuilder));
        }
    }
}
