using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Common.Infrastructure.SftpClient;
using Xunit;

namespace Telemedicine.Common.Infrastructure.Tests.SftpClientTests.Validators
{
    public class SftpClientSettingsValidatorTests
    {
        private readonly SftpClientSettings _defaultModel;
        private readonly SftpClientSettingsValidator _validator;

        public SftpClientSettingsValidatorTests()
        {
            _defaultModel = new SftpClientSettings();
            _validator = new SftpClientSettingsValidator();
        }

        [Fact]
        public async Task EmptyModel_ShouldBeCorrect()
        {
            // Act
            var result = await _validator.TestValidateAsync(_defaultModel);

            // Assert
            result.ShouldHaveValidationErrorFor(_ => _.Host);
            result.ShouldNotHaveValidationErrorFor(_ => _.Port);
            result.ShouldNotHaveValidationErrorFor(_ => _.KeepAliveIntervalInSeconds);
            result.ShouldHaveValidationErrorFor(_ => _.Username);
            result.ShouldHaveValidationErrorFor(_ => _.PrivateKey);
            result.ShouldNotHaveValidationErrorFor(_ => _.PassPhrase);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("example.com", true)]
        [InlineData("127.0.0.1", true)]
        [InlineData("localhost", true)]
        public async Task Hostname_ShouldBeCorrect(string hostname, bool isValid)
        {
            // Act
            var result = await _validator.TestValidateAsync(_defaultModel with { Host = hostname });

            // Assert
            if (isValid)
            {
                result.ShouldNotHaveValidationErrorFor(_ => _.Host);
            }
            else
            {
                result.ShouldHaveValidationErrorFor(_ => _.Host);
            }
        }

        [Theory]
        [InlineData(-1, false)]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(int.MaxValue, false)]
        public async Task Port_ShouldBeCorrect(int port, bool isValid)
        {
            // Act
            var result = await _validator.TestValidateAsync(_defaultModel with { Port = port });

            // Assert
            if (isValid)
            {
                result.ShouldNotHaveValidationErrorFor(_ => _.Port);
            }
            else
            {
                result.ShouldHaveValidationErrorFor(_ => _.Port);
            }
        }

        [Theory]
        [InlineData(int.MinValue, false)]
        [InlineData(-1, true)]
        [InlineData(0, true)]
        [InlineData(1, true)]
        [InlineData(int.MaxValue, true)]
        public async Task KeepAliveIntervalInSeconds_ShouldBeCorrect(int keepAliveIntervalInSeconds, bool isValid)
        {
            // Act
            var result = await _validator.TestValidateAsync(_defaultModel with { KeepAliveIntervalInSeconds = keepAliveIntervalInSeconds });

            // Assert
            if (isValid)
            {
                result.ShouldNotHaveValidationErrorFor(_ => _.KeepAliveIntervalInSeconds);
            }
            else
            {
                result.ShouldHaveValidationErrorFor(_ => _.KeepAliveIntervalInSeconds);
            }
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("username", true)]
        public async Task Username_ShouldBeCorrect(string username, bool isValid)
        {
            // Act
            var result = await _validator.TestValidateAsync(_defaultModel with { Username = username });

            // Assert
            if (isValid)
            {
                result.ShouldNotHaveValidationErrorFor(_ => _.Username);
            }
            else
            {
                result.ShouldHaveValidationErrorFor(_ => _.Username);
            }
        }

        [Theory]
        [InlineData("-----BEGIN RSA PRIVATE KEY-----\r\n" +
                    "MIIBOAIBAAJATS3iHq7SiSKNjEVmvBkLyxaej1rYH7BFfKE/oQCR9xPMhfHxg4Am\r\n" +
                    "7E/34mmjkK8IO86iy+9TnbRNeESRfoSe/wIDAQABAkAabNZaGRN+3rTGTVDioFS4\r\n" +
                    "YXNCtCEBoJH6HR+zGYXgqac9KXXQ6VtE/hKs780v8eEdXKaSLcnlyLF7E+gipJ75\r\n" +
                    "AiEAl2KmCVM4hgp7CTFI7RsTp6XTNik7Ts5IL5PZdVmVl3MCIQCCg4s20xdssdqv\r\n" +
                    "qJ9zXiCHE3sIEdqowhzv87aKuPy/RQIgX8Muhv2W5uvgUrtTh3G2aDm0tzEz7DD/\r\n" +
                    "ZuFfESW/RFMCIA5F0nChbxtc69xv5VgZgBclgzvjr/TqnX2EOn87MbC9AiA9aLGh\r\n" +
                    "zWKDSSTmVJy+0xwggszz7wWHeK8G48PVcB1NoQ==\r\n" +
                    "-----END RSA PRIVATE KEY-----", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public async Task PrivateKey_ShouldBeCorrect(string privateKey, bool isValid)
        {
            // Act
            var result = await _validator.TestValidateAsync(_defaultModel with { PrivateKey = privateKey });

            // Assert
            if (isValid)
            {
                result.ShouldNotHaveValidationErrorFor(_ => _.PrivateKey);
            }
            else
            {
                result.ShouldHaveValidationErrorFor(_ => _.PrivateKey);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("PassPhrase")]
        public async Task PassPhrase_ShouldBeCorrect(string passPhrase)
        {
            // Act
            var result = await _validator.TestValidateAsync(_defaultModel with { PassPhrase = passPhrase });

            // Assert
            result.ShouldNotHaveValidationErrorFor(_ => _.PassPhrase);
        }
    }
}
