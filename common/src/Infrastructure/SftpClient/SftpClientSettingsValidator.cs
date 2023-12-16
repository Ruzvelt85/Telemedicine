using System;
using System.Runtime.CompilerServices;
using FluentValidation;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Validators;
using Telemedicine.Common.Infrastructure.SftpClient.Extensions;
using Renci.SshNet;

[assembly: InternalsVisibleTo("Telemedicine.Common.Infrastructure.Tests.SftpClientTests")]

namespace Telemedicine.Common.Infrastructure.SftpClient
{
    internal class SftpClientSettingsValidator : AbstractSettingsValidator<SftpClientSettings>
    {
        public SftpClientSettingsValidator()
        {
            RuleFor(_ => _.Host).NotEmpty();
            RuleFor(_ => _.Port).InclusiveBetween(1, 65535);
            RuleFor(_ => _.KeepAliveIntervalInSeconds).GreaterThanOrEqualTo(-1);
            RuleFor(_ => _.Username).NotEmpty();
            RuleFor(_ => _.PrivateKey).Custom((privateKey, context) =>
            {
                try
                {
                    using var stream = privateKey.GetStream();
                    var _ = new PrivateKeyFile(stream);
                }
                catch (Exception ex)
                {
                    context.AddFailure($"'{context.DisplayName}' {ex.Message}");
                }
            });
        }
    }
}
