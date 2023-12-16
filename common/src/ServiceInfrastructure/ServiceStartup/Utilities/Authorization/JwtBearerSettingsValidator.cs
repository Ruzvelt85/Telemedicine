using FluentValidation;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Settings;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities.Authorization
{
    internal class JwtBearerSettingsValidator : AbstractValidator<JwtBearerSettings>
    {
        public JwtBearerSettingsValidator()
        {
            RuleFor(_ => _.Authority).NotEmpty();
            RuleFor(_ => _.TokenValidationParameters)
                .SetValidator(new TokenValidationParametersValidator()!)
                .When(_ => _.TokenValidationParameters is not null); //this check is redundant here but makes the condition clearer IMO
        }
    }

    internal class TokenValidationParametersValidator : AbstractValidator<TokenValidationParameters>
    {
        public TokenValidationParametersValidator()
        {
            RuleFor(_ => _.ValidateLifetime).NotNull();
        }
    }
}
