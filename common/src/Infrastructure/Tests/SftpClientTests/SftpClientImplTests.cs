using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using FluentAssertions;
using Telemedicine.Common.Infrastructure.SftpClient;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Telemedicine.Common.Infrastructure.Tests.SftpClientTests
{
    public class SftpClientImplTests
    {
        [Fact]
        public void Ctor_WhenFactoryIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ExternalSftpClientFactory factory = default!;
            var optionsMonitor = new Mock<IOptionsMonitor<SftpClientSettings>>();
            optionsMonitor.Setup(_ => _.CurrentValue);
            const string expectedParameterName = "sftpClientFactory";

            // Act
            Func<SftpClientImpl> func = () => new SftpClientImpl(factory, optionsMonitor.Object);

            // Assert
            func.Should().Throw<ArgumentNullException>()
                .WithParameterName(expectedParameterName);
        }

        [Fact]
        public void Ctor_WhenOptionsMonitorIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var factory = new ExternalSftpClientFactory();
            IOptionsMonitor<SftpClientSettings> optionsMonitor = default!;
            const string expectedParameterName = "sftpClientOptionsMonitor";

            // Act
            Func<SftpClientImpl> func = () => new SftpClientImpl(factory, optionsMonitor);

            // Assert
            func.Should().Throw<ArgumentNullException>()
                .WithParameterName(expectedParameterName);
        }

        [Fact]
        public void Dispose_FirstTimeCalled_ShouldNotThrow()
        {
            // Arrange
            var factory = new ExternalSftpClientFactory();
            var settings = GetValidSftpClientSettings();

            var optionsMonitor = new Mock<IOptionsMonitor<SftpClientSettings>>();
            optionsMonitor.Setup(_ => _.CurrentValue).Returns(settings);
            var sftpClient = new SftpClientImpl(factory, optionsMonitor.Object);

            // Act
            Action action = () => sftpClient.Dispose();

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Dispose_CalledOnDisposedObject_ShouldNotThrow()
        {
            // Arrange
            var factory = new ExternalSftpClientFactory();
            var settings = GetValidSftpClientSettings();

            var optionsMonitor = new Mock<IOptionsMonitor<SftpClientSettings>>();
            optionsMonitor.Setup(_ => _.CurrentValue).Returns(settings);
            var sftpClient = new SftpClientImpl(factory, optionsMonitor.Object);

            sftpClient.Dispose();

            // Act
            Action action = () => sftpClient.Dispose();

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void ListDirectory_WhenDisposed_ShouldThrow()
        {
            // Arrange
            var factory = new ExternalSftpClientFactory();
            var settings = GetValidSftpClientSettings();

            var optionsMonitor = new Mock<IOptionsMonitor<SftpClientSettings>>();
            optionsMonitor.Setup(_ => _.CurrentValue).Returns(settings);
            var sftpClient = new SftpClientImpl(factory, optionsMonitor.Object);
            Trace.Listeners.Clear();
            sftpClient.Dispose();

            // Act
            Func<IEnumerable<string>> func = () => sftpClient.ListDirectory(string.Empty).ToList();

            // Assert
            func.Should().Throw<ObjectDisposedException>();
        }

        [Fact]
        public void OpenRead_WhenDisposed_ShouldThrow()
        {
            // Arrange
            var factory = new ExternalSftpClientFactory();
            var settings = GetValidSftpClientSettings();

            var optionsMonitor = new Mock<IOptionsMonitor<SftpClientSettings>>();
            optionsMonitor.Setup(_ => _.CurrentValue).Returns(settings);
            var sftpClient = new SftpClientImpl(factory, optionsMonitor.Object);
            Trace.Listeners.Clear();
            sftpClient.Dispose();

            // Act
            Func<StreamReader> func = () => sftpClient.OpenRead("file.name");

            // Assert
            func.Should().Throw<ObjectDisposedException>();
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
