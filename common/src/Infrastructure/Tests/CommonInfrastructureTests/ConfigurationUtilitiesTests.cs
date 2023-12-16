using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Unexpected;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.Testing;
using Telemedicine.Common.Infrastructure.Tests.CommonInfrastructureTests.Settings;
using Telemedicine.Common.Infrastructure.Tests.CommonInfrastructureTests.Validators;

namespace Telemedicine.Common.Infrastructure.Tests.CommonInfrastructureTests
{
    public class ConfigurationUtilitiesTests : UnitTestsBase
    {
        [Fact]
        public void GetSettings_WhenSettingsExists_ShouldReturnCorrect()
        {
            // Arrange
            var configurationDictionary = new Dictionary<string, string>
            {
                { $"{nameof(TestSettings)}:{nameof(TestSettings.IsEnabled)}", bool.FalseString },
                { $"{nameof(TestSettings)}:{nameof(TestSettings.Property)}", "PropertyValue" }
            };
            var configuration = GetConfiguration(configurationDictionary);

            // Act
            var settings = configuration.GetSettings<TestSettings>();

            // Assert
            Assert.NotNull(settings);
            Assert.IsType<TestSettings>(settings);
            Assert.False(settings!.IsEnabled);
            Assert.Equal("PropertyValue", settings.Property);
        }

        [Fact]
        public void GetSettings_WhenSettingsNotExists_ShouldBeNull()
        {
            // Arrange
            var configuration = GetConfiguration(new Dictionary<string, string>());

            // Act
            var settings = configuration.GetSettings<TestSettings>();

            // Assert
            Assert.Null(settings);
        }

        [Fact]
        public void GetSettingsAndValidate_WhenSettingsExistsAndValid_ShouldReturnCorrectSettings()
        {
            // Arrange
            var configurationDictionary = new Dictionary<string, string>
            {
                { $"{nameof(TestSettingsWithValidation)}:{nameof(TestSettingsWithValidation.IsEnabled)}", bool.FalseString },
                { $"{nameof(TestSettingsWithValidation)}:{nameof(TestSettingsWithValidation.Property)}", "PropertyValue" }
            };
            var configuration = GetConfiguration(configurationDictionary);

            // Act
            var settings = configuration.GetSettingsAndValidate<TestSettingsWithValidation, TestSettingsWithValidationValidator>();

            // Assert
            Assert.NotNull(settings);
            Assert.IsType<TestSettingsWithValidation>(settings);
            Assert.False(settings.IsEnabled);
            Assert.Equal("PropertyValue", settings.Property);
        }

        [Fact]
        public void GetSettingsAndValidate_WhenSettingsExistsAndInvalid_ShouldThrowException()
        {
            // Arrange
            var configurationDictionary = new Dictionary<string, string>
            {
                { $"{nameof(TestSettingsWithValidation)}:{nameof(TestSettingsWithValidation.IsEnabled)}", bool.FalseString },
                { $"{nameof(TestSettingsWithValidation)}:{nameof(TestSettingsWithValidation.Property)}", string.Empty }
            };
            var configuration = GetConfiguration(configurationDictionary);

            // Act
            var exception = Record.Exception(() => configuration.GetSettingsAndValidate<TestSettingsWithValidation, TestSettingsWithValidationValidator>());

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ConfigurationValidationException>(exception);

            var customException = exception as ConfigurationValidationException;
            Assert.Equal(nameof(TestSettingsWithValidation), customException!.ConfigurationName);
        }

        [Fact]
        public void GetSettingsAndValidate_WhenSettingsExistsAndInvalid_PropertyIsNull_ShouldThrowException()
        {
            // Arrange
            var configurationDictionary = new Dictionary<string, string>
            {
                { $"{nameof(TestSettingsWithValidation)}:{nameof(TestSettingsWithValidation.IsEnabled)}", bool.FalseString }
            };
            var configuration = GetConfiguration(configurationDictionary);

            // Act
            var exception = Record.Exception(() => configuration.GetSettingsAndValidate<TestSettingsWithValidation, TestSettingsWithValidationValidator>());

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ConfigurationValidationException>(exception);

            var customException = exception as ConfigurationValidationException;
            Assert.Equal(nameof(TestSettingsWithValidation), customException!.ConfigurationName);
        }

        [Fact]
        public void GetSettingsAndValidate_WhenSettingsNotExistsAndDefault_ValidationSkipped_ShouldThrowException()
        {
            // Arrange
            var configuration = GetConfiguration(new Dictionary<string, string>());

            // Act
            var exception = Record.Exception(() => configuration.GetSettingsAndValidate<TestSettingsWithValidation, TestSettingsWithValidationValidator>());

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ConfigurationMissingException>(exception);

            var customException = exception as ConfigurationMissingException;
            Assert.Equal(nameof(TestSettingsWithValidation), customException!.ConfigurationName);
        }

        [Fact]
        public void ConfigureSettings_WhenSettingsExists_ShouldReturnCorrect()
        {
            // Arrange
            var configurationDictionary = new Dictionary<string, string>
            {
                { $"{nameof(TestSettings)}:{nameof(TestSettings.IsEnabled)}", bool.FalseString },
                { $"{nameof(TestSettings)}:{nameof(TestSettings.Property)}", "PropertyValue" }
            };
            var configuration = GetConfiguration(configurationDictionary);
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.ConfigureSettings<TestSettings>(configuration);
            var testSettingsOptions = serviceCollection.BuildServiceProvider().GetRequiredService<IOptions<TestSettings>>();

            // Assert
            Assert.NotNull(testSettingsOptions);
            Assert.IsType<TestSettings>(testSettingsOptions.Value);
            Assert.False(testSettingsOptions.Value!.IsEnabled);
            Assert.Equal("PropertyValue", testSettingsOptions.Value.Property);
        }

        [Fact]
        public void ConfigureSettings_WhenSettingsNotExists_ShouldThrowException()
        {
            // Arrange
            var configuration = GetConfiguration(new Dictionary<string, string>());
            var serviceCollection = new ServiceCollection();

            // Act
            var exception = Record.Exception(() => serviceCollection.ConfigureSettings<TestSettings>(configuration));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ConfigurationMissingException>(exception);
            var customException = exception as ConfigurationMissingException;
            Assert.Equal(nameof(TestSettings), customException!.ConfigurationName);
        }

        [Fact]
        public void ConfigureSettingsWithValidation_WhenSettingsExistsAndValid_ShouldReturnCorrect()
        {
            // Arrange
            var configurationDictionary = new Dictionary<string, string>
            {
                { $"{nameof(TestSettingsWithValidation)}:{nameof(TestSettingsWithValidation.IsEnabled)}", bool.FalseString },
                { $"{nameof(TestSettingsWithValidation)}:{nameof(TestSettingsWithValidation.Property)}", "PropertyValue" }
            };
            var configuration = GetConfiguration(configurationDictionary);
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.ConfigureSettings<TestSettingsWithValidation, TestSettingsWithValidationValidator>(configuration);
            var testSettingsOptions = serviceCollection.BuildServiceProvider().GetRequiredService<IOptions<TestSettingsWithValidation>>();

            // Assert
            Assert.NotNull(testSettingsOptions);
            Assert.IsType<TestSettingsWithValidation>(testSettingsOptions.Value);
            Assert.False(testSettingsOptions.Value!.IsEnabled);
            Assert.Equal("PropertyValue", testSettingsOptions.Value.Property);
        }

        [Fact]
        public void ConfigureSettingsWithValidation_WhenSettingsExistsAndNotValid_ShouldThrowException()
        {
            // Arrange
            var configurationDictionary = new Dictionary<string, string>
            {
                { $"{nameof(TestSettingsWithValidation)}:{nameof(TestSettingsWithValidation.Property)}", string.Empty }
            };
            var configuration = GetConfiguration(configurationDictionary);
            var serviceCollection = new ServiceCollection();

            // Act
            var exception = Record.Exception(() => serviceCollection.ConfigureSettings<TestSettingsWithValidation, TestSettingsWithValidationValidator>(configuration));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ConfigurationValidationException>(exception);
            var customException = exception as ConfigurationValidationException;
            Assert.Equal(nameof(TestSettingsWithValidation), customException!.ConfigurationName);
        }

        [Fact]
        public void ConfigureSettingsWithValidation_WhenSettingsNotExists_ShouldThrowException()
        {
            // Arrange
            var configuration = GetConfiguration(new Dictionary<string, string>());
            var serviceCollection = new ServiceCollection();

            // Act
            var exception = Record.Exception(
                () => serviceCollection.ConfigureSettings<TestSettingsWithValidation, TestSettingsWithValidationValidator>(configuration));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ConfigurationMissingException>(exception);
            var customException = exception as ConfigurationMissingException;
            Assert.Equal(nameof(TestSettingsWithValidation), customException!.ConfigurationName);
        }

        [Fact]
        public void GetDictionarySettings_WhenSettingsExists_ShouldReturnCorrect()
        {
            // Arrange
            var configurationList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>($"{nameof(TestSettings)}:Key1:{nameof(TestSettings.IsEnabled)}", bool.TrueString),
                new KeyValuePair<string, string>($"{nameof(TestSettings)}:Key1:{nameof(TestSettings.Property)}", "PropertyValue1"),
                new KeyValuePair<string, string>($"{nameof(TestSettings)}:Key2:{nameof(TestSettings.IsEnabled)}", bool.FalseString),
                new KeyValuePair<string, string>($"{nameof(TestSettings)}:Key2:{nameof(TestSettings.Property)}", "PropertyValue2")
            };
            var configuration = GetConfiguration(configurationList);

            // Act
            var settings = configuration.GetDictionarySettings<TestSettings>();

            // Assert
            Assert.NotNull(settings);
            Assert.IsType<Dictionary<string, TestSettings>>(settings);
            Assert.True(settings!["Key1"].IsEnabled);
            Assert.Equal("PropertyValue1", settings["Key1"].Property);
            Assert.False(settings!["Key2"].IsEnabled);
            Assert.Equal("PropertyValue2", settings["Key2"].Property);
        }

        [Fact]
        public void GetDictionarySettings_WhenSettingsNotExists_ShouldBeNull()
        {
            // Arrange
            var configuration = GetConfiguration(new List<KeyValuePair<string, string>>());

            // Act
            var settings = configuration.GetDictionarySettings<TestSettings>();

            // Assert
            Assert.Null(settings);
        }

        [Fact]
        public void ConfigureDictionarySettings_WhenSettingsExists_ShouldReturnCorrect()
        {
            // Arrange
            var configurationList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>($"{nameof(TestSettings)}:Key1:{nameof(TestSettings.IsEnabled)}", bool.TrueString),
                new KeyValuePair<string, string>($"{nameof(TestSettings)}:Key1:{nameof(TestSettings.Property)}", "PropertyValue1"),
                new KeyValuePair<string, string>($"{nameof(TestSettings)}:Key2:{nameof(TestSettings.IsEnabled)}", bool.FalseString),
                new KeyValuePair<string, string>($"{nameof(TestSettings)}:Key2:{nameof(TestSettings.Property)}", "PropertyValue2")
            };
            var configuration = GetConfiguration(configurationList);
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.ConfigureDictionarySettings<TestSettings>(configuration);
            var testSettingsOptions = serviceCollection.BuildServiceProvider().GetRequiredService<IOptions<Dictionary<string, TestSettings>>>();

            // Assert
            Assert.NotNull(testSettingsOptions);
            Assert.IsType<Dictionary<string, TestSettings>>(testSettingsOptions.Value);
            Dictionary<string, TestSettings> settings = testSettingsOptions.Value;
            Assert.True(settings["Key1"].IsEnabled);
            Assert.Equal("PropertyValue1", settings["Key1"].Property);
            Assert.False(settings["Key2"].IsEnabled);
            Assert.Equal("PropertyValue2", settings["Key2"].Property);
        }

        [Fact]
        public void ConfigureDictionarySettings_WhenSettingsNotExistsNotAllowEmpty_ShouldThrowException()
        {
            // Arrange
            var configuration = GetConfiguration(new Dictionary<string, string>());
            var serviceCollection = new ServiceCollection();

            // Act
            var exception = Record.Exception(
                () => serviceCollection.ConfigureDictionarySettings<TestSettingsWithValidation>(configuration));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ConfigurationMissingException>(exception);
            var customException = exception as ConfigurationMissingException;
            Assert.Equal(nameof(TestSettingsWithValidation), customException!.ConfigurationName);
        }

        [Fact]
        public void ConfigureDictionarySettings_WhenSettingsNotExistsAllowEmpty_ShouldBeEmpty()
        {
            // Arrange
            var configuration = GetConfiguration(new Dictionary<string, string>());
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.ConfigureDictionarySettings<TestSettingsWithValidation>(configuration, true);

            var testSettingsOptions = serviceCollection.BuildServiceProvider().GetRequiredService<IOptions<Dictionary<string, TestSettings>>>();

            // Assert
            Assert.NotNull(testSettingsOptions);
            Assert.IsType<Dictionary<string, TestSettings>>(testSettingsOptions.Value);
            Assert.Empty(testSettingsOptions.Value);
        }

        private static IConfigurationRoot GetConfiguration(IEnumerable<KeyValuePair<string, string>>? myConfiguration)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();
        }
    }
}
