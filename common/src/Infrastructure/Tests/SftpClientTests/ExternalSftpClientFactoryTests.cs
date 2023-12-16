using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Unexpected;
using Telemedicine.Common.Contracts.GlobalContracts.Utilities;
using Telemedicine.Common.Infrastructure.SftpClient;
using Telemedicine.Common.Infrastructure.SftpClient.Exceptions;
using Xunit;

namespace Telemedicine.Common.Infrastructure.Tests.SftpClientTests
{
    public class ExternalSftpClientFactoryTests
    {
        [Fact]
        public void Create_WhenSettingsAreValid_ShouldBuild()
        {
            // Arrange
            var settings = GetValidSftpClientSettings();
            var factory = new ExternalSftpClientFactory();

            // Act
            var externalSftpClient = factory.Create(settings);

            // Assert
            externalSftpClient.Should().NotBeNull();
            externalSftpClient.KeepAliveInterval.Should().Be(TimeSpan.FromSeconds(settings.KeepAliveIntervalInSeconds));
            externalSftpClient.ConnectionInfo.Host.Should().Be(settings.Host);
            externalSftpClient.ConnectionInfo.Port.Should().Be(settings.Port);
            externalSftpClient.ConnectionInfo.Username.Should().Be(settings.Username);
        }

        [Fact]
        public void Create_WhenPrivateKey_ShouldThrowInitializeSftpClientException()
        {
            // Arrange
            var factory = new ExternalSftpClientFactory();
            var settings = GetValidSftpClientSettings() with { PrivateKey = string.Empty };

            // Act
            Action action = () => factory.Create(settings);

            // Assert
            action.Should().Throw<InitializeSftpClientException>();
        }

        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        private static SftpClientSettings GetValidSftpClientSettings() =>
            new()
            {
                Host = "sftp.test.com",
                Port = 22,
                KeepAliveIntervalInSeconds = 10,
                Username = "test_user",
                PrivateKey = "-----BEGIN RSA PRIVATE KEY-----\r\n" +
                             "MIIBOAIBAAJATS3iHq7SiSKNjEVmvBkLyxaej1rYH7BFfKE/oQCR9xPMhfHxg4Am\r\n" +
                             "7E/34mmjkK8IO86iy+9TnbRNeESRfoSe/wIDAQABAkAabNZaGRN+3rTGTVDioFS4\r\n" +
                             "YXNCtCEBoJH6HR+zGYXgqac9KXXQ6VtE/hKs780v8eEdXKaSLcnlyLF7E+gipJ75\r\n" +
                             "AiEAl2KmCVM4hgp7CTFI7RsTp6XTNik7Ts5IL5PZdVmVl3MCIQCCg4s20xdssdqv\r\n" +
                             "qJ9zXiCHE3sIEdqowhzv87aKuPy/RQIgX8Muhv2W5uvgUrtTh3G2aDm0tzEz7DD/\r\n" +
                             "ZuFfESW/RFMCIA5F0nChbxtc69xv5VgZgBclgzvjr/TqnX2EOn87MbC9AiA9aLGh\r\n" +
                             "zWKDSSTmVJy+0xwggszz7wWHeK8G48PVcB1NoQ==\r\n" +
                             "-----END RSA PRIVATE KEY-----"
            };
    }

}
