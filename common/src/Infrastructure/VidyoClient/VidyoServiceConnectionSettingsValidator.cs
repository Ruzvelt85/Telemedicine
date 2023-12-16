using FluentValidation;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Validators;

namespace Telemedicine.Common.Infrastructure.VidyoClient
{
    internal class VidyoServiceConnectionSettingsValidator : AbstractSettingsValidator<VidyoServiceConnectionSettings>
    {
        public VidyoServiceConnectionSettingsValidator()
        {
            RuleFor(_ => _.Url).NotEmpty();
            RuleFor(_ => _.UserName).NotEmpty();
            RuleFor(_ => _.UserPassword).NotEmpty();
            RuleFor(_ => _.Timeout).GreaterThan(0);
        }
    }
}
