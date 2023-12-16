using FluentValidation;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Validators;

namespace Telemedicine.Services.VideoConfIntegrService.WebAPI.Services
{
    internal class ConferenceSettingsValidator : AbstractSettingsValidator<ConferenceSettings>
    {
        public ConferenceSettingsValidator()
        {
            RuleFor(_ => _.ExtensionPrefix).NotEmpty();
            RuleFor(_ => _.PinCodeFormat).Must(x => x is >= 6 and <= 12).NotEmpty();
        }
    }
}
