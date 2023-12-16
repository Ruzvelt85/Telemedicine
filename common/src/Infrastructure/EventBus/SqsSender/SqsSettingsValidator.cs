using FluentValidation;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Validators;

namespace Telemedicine.Common.Infrastructure.EventBus.SqsSender
{
    public class SqsSettingsValidator : AbstractSettingsValidator<SqsSettings>
    {
        public SqsSettingsValidator()
        {
            RuleFor(_ => _.Url).NotEmpty();
            RuleFor(_ => _.IsFifo).NotNull();
            RuleFor(_ => _.AccessKey).NotEmpty();
            RuleFor(_ => _.SecretKey).NotEmpty();
            RuleFor(_ => _.AmazonConfiguration).Cascade(CascadeMode.Stop).NotNull().SetValidator(new AmazonSqsClientSettingsValidator()!);
        }
    }

    internal class AmazonSqsClientSettingsValidator : AbstractValidator<SqsSettings.AmazonSqsClientSettings>
    {
        public AmazonSqsClientSettingsValidator()
        {
            RuleFor(_ => _.RegionEndpoint).NotEmpty();
            /* all other fields are not mandatory*/
        }
    }
}
