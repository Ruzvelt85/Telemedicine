using FluentValidation;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Validators;
using Telemedicine.Common.Infrastructure.Tests.CommonInfrastructureTests.Settings;

namespace Telemedicine.Common.Infrastructure.Tests.CommonInfrastructureTests.Validators
{
    class TestSettingsWithValidationValidator : AbstractSettingsValidator<TestSettingsWithValidation>
    {
        public TestSettingsWithValidationValidator()
        {
            RuleFor(_ => _.Property).NotEmpty();
        }
    }
}
