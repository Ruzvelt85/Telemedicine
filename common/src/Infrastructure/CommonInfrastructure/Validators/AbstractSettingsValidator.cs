using FluentValidation;
using Microsoft.Extensions.Options;
using Serilog;

namespace Telemedicine.Common.Infrastructure.CommonInfrastructure.Validators
{
    public class AbstractSettingsValidator<T> : AbstractValidator<T>, IValidateOptions<T> where T : class
    {
        private readonly ILogger _logger = Log.ForContext<AbstractSettingsValidator<T>>();

        public ValidateOptionsResult Validate(string name, T options)
        {
            var validationResult = base.Validate(options);

            if (validationResult.IsValid)
            {
                _logger.Debug("Configuration for '{Setting}' is valid.", typeof(T));
                return ValidateOptionsResult.Success;
            }

            _logger.Warning("Configuration for '{Setting}' is not valid. Errors: {ValidationResult}", typeof(T), validationResult);
            return ValidateOptionsResult.Fail($"Configuration for '{typeof(T)}' is not valid. Errors: {validationResult}");
        }
    }
}
