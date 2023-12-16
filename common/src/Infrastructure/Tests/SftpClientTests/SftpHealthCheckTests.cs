using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Telemedicine.Common.Infrastructure.SftpClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Telemedicine.Common.Infrastructure.Tests.SftpClientTests
{
    public class SftpHealthCheckTests
    {
        private const HealthStatus FailureStatus = HealthStatus.Degraded;
        private const string Name = "health_check_name";

        [Fact]
        public async Task CheckHealthAsync_WhenCannotCreateSftpClientFactory_ShouldReturnFailureStatus()
        {
            // Arrange
            var settings = GetValidSftpClientSettings() with
            {
                Host = $"{nameof(CheckHealthAsync_WhenCannotCreateSftpClientFactory_ShouldReturnFailureStatus)}.sftp.test.com"
            };

            var optionsMonitor = new Mock<IOptionsMonitor<SftpClientSettings>>();
            optionsMonitor.Setup(_ => _.CurrentValue).Returns(settings);

            ISftpClient sftpClient = default!;

            var sftpHealthCheck = new SftpHealthCheck(optionsMonitor.Object, (_) => sftpClient);
            var registration = new HealthCheckRegistration(Name, sftpHealthCheck, FailureStatus, Enumerable.Empty<string>(), null);
            var healthCheckContext = new HealthCheckContext() { Registration = registration };

            // Act
            var result = await sftpHealthCheck.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(FailureStatus);
            result.Exception.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public async Task CheckHealthAsync_WhenSftpClientCannotConnect_ShouldReturnFailureStatus()
        {
            // Arrange
            var settings = GetValidSftpClientSettings() with
            {
                Host = $"{nameof(CheckHealthAsync_WhenSftpClientCannotConnect_ShouldReturnFailureStatus)}.sftp.test.com"
            };

            var optionsMonitor = new Mock<IOptionsMonitor<SftpClientSettings>>();
            optionsMonitor.Setup(_ => _.CurrentValue).Returns(settings);

            var expectedException = new Exception();
            var sftpClientMock = new Mock<ISftpClient>();
            sftpClientMock.Setup(_ => _.TryConnect()).Throws(expectedException);

            var sftpHealthCheck = new SftpHealthCheck(optionsMonitor.Object, _ => sftpClientMock.Object);
            var registration = new HealthCheckRegistration(Name, sftpHealthCheck, FailureStatus, Enumerable.Empty<string>(), null);
            var healthCheckContext = new HealthCheckContext() { Registration = registration };

            // Act
            var result = await sftpHealthCheck.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(FailureStatus);
            result.Exception.Should().Be(expectedException);
        }

        [Fact]
        public async Task CheckHealthAsync_WhenSftpClientIsNotConnected_ShouldReturnFailureStatus()
        {
            // Arrange
            var settings = GetValidSftpClientSettings() with
            {
                Host = $"{nameof(CheckHealthAsync_WhenSftpClientIsNotConnected_ShouldReturnFailureStatus)}.sftp.test.com"
            };

            var optionsMonitor = new Mock<IOptionsMonitor<SftpClientSettings>>();
            optionsMonitor.Setup(_ => _.CurrentValue).Returns(settings);

            var sftpClientMock = new Mock<ISftpClient>();
            sftpClientMock.Setup(_ => _.TryConnect()).Returns(false);

            var sftpHealthCheck = new SftpHealthCheck(optionsMonitor.Object, (_) => sftpClientMock.Object);
            var registration = new HealthCheckRegistration(Name, sftpHealthCheck, FailureStatus, Enumerable.Empty<string>(), null);
            var healthCheckContext = new HealthCheckContext() { Registration = registration };

            // Act
            var result = await sftpHealthCheck.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(FailureStatus);
        }

        [Fact]
        public async Task CheckHealthAsync_WhenSftpClientIsHealthy_ShouldReturnHealthStatus()
        {
            // Arrange
            var settings = GetValidSftpClientSettings() with
            {
                Host = $"{nameof(CheckHealthAsync_WhenSftpClientIsHealthy_ShouldReturnHealthStatus)}.sftp.test.com"
            };

            var optionsMonitor = new Mock<IOptionsMonitor<SftpClientSettings>>();
            optionsMonitor.Setup(_ => _.CurrentValue).Returns(settings);

            var sftpClientMock = new Mock<ISftpClient>();
            sftpClientMock.Setup(_ => _.TryConnect()).Returns(true);

            var sftpHealthCheck = new SftpHealthCheck(optionsMonitor.Object, (_) => sftpClientMock.Object);
            var registration = new HealthCheckRegistration(Name, sftpHealthCheck, HealthStatus.Degraded, Enumerable.Empty<string>(), null);
            var healthCheckContext = new HealthCheckContext() { Registration = registration };

            // Act
            var result = await sftpHealthCheck.CheckHealthAsync(healthCheckContext, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(HealthStatus.Healthy);
        }

        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        private static SftpClientSettings GetValidSftpClientSettings() =>
            new()
            {
                Host = "sftp.test.com",
                Port = 22,
                Username = "test_user",
                PrivateKey = "-----BEGIN RSA PRIVATE KEY-----\r\nMIIBOAIBAAJATS3iHq7SiSKNjEVmvBkLyxaej1rYH7BFfKE/oQCR9xPMhfHxg4Am\r\n7E/34mmjkK8IO86iy+9TnbRNeESRfoSe/wIDAQABAkAabNZaGRN+3rTGTVDioFS4\r\nYXNCtCEBoJH6HR+zGYXgqac9KXXQ6VtE/hKs780v8eEdXKaSLcnlyLF7E+gipJ75\r\nAiEAl2KmCVM4hgp7CTFI7RsTp6XTNik7Ts5IL5PZdVmVl3MCIQCCg4s20xdssdqv\r\nqJ9zXiCHE3sIEdqowhzv87aKuPy/RQIgX8Muhv2W5uvgUrtTh3G2aDm0tzEz7DD/\r\nZuFfESW/RFMCIA5F0nChbxtc69xv5VgZgBclgzvjr/TqnX2EOn87MbC9AiA9aLGh\r\nzWKDSSTmVJy+0xwggszz7wWHeK8G48PVcB1NoQ==\r\n-----END RSA PRIVATE KEY-----"
            };
    }
}
