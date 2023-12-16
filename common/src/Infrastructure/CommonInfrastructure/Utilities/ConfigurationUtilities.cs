using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Unexpected;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Validators;

namespace Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities
{
    /// <summary>
    /// Utility methods to work with configuration settings
    /// </summary>
    [PublicAPI]
    public static class ConfigurationUtilities
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(ConfigurationUtilities));

        /// <summary>
        /// Registers settings with settings validator into IoC and validate settings at startup
        /// </summary>
        /// <typeparam name="TSetting">Settings type</typeparam>
        /// <typeparam name="TValidator">Validator type</typeparam>
        /// <param name="configuration">Configuration provider</param>
        /// <param name="services">Collections of services</param>
        /// <exception cref="ConfigurationMissingException">Thrown if getting of settings instance was unsuccessful and default value is <c>null</c></exception>
        /// <exception cref="ConfigurationValidationException">Thrown if validation of settings instance was unsuccessful</exception>
        public static IServiceCollection ConfigureSettings<TSetting, TValidator>(this IServiceCollection services, IConfiguration configuration)
            where TValidator : AbstractSettingsValidator<TSetting>, new()
            where TSetting : class
        {
            configuration.GetSettingsAndValidate<TSetting, TValidator>();

            services.ConfigureSettings<TSetting>(configuration);
            services.AddSingleton<IValidateOptions<TSetting>, TValidator>();

            return services;
        }

        /// <summary>
        /// Registries settings into IoC
        /// </summary>
        /// <typeparam name="T">setting class type</typeparam>
        /// <param name="services">IoC service collection</param>
        /// <param name="configuration">Configuration provider</param>
        public static IServiceCollection ConfigureSettings<T>(this IServiceCollection services, IConfiguration configuration) where T : class
        {
            var settings = configuration.GetSettings<T>();
            ThrowExceptionIfNull(settings);
            _logger.Information("The following settings have been used for the {@SettingsName}: {@Settings}", typeof(T).Name, settings);
            return services.ConfigureSettings(configuration, typeof(T));
        }

        /// <summary>
        /// Registries collection settings into IoC
        /// </summary>
        /// <typeparam name="T">setting class type</typeparam>
        /// <param name="services">IoC service collection</param>
        /// <param name="configuration">Configuration provider</param>
        /// <param name="allowEmptySection">Allow configure on empty or absent section</param>
        /// <exception cref="ConfigurationMissingException">Thrown if getting of settings section if empty ar absent.</exception>
        public static IServiceCollection ConfigureDictionarySettings<T>(this IServiceCollection services, IConfiguration configuration, bool allowEmptySection = false)
            where T : class
        {
            Dictionary<string, T>? settings = configuration.GetDictionarySettings<T>();
            string settingsName = typeof(T).Name;
            if (!allowEmptySection)
            { ThrowExceptionIfNull(settings, settingsName); }

            _logger.Information("The following settings have been used for the {@SettingsName}: {@Settings}", settingsName, settings);
            return services.ConfigureSettings(configuration, typeof(Dictionary<string, T>), typeof(T).Name);
        }

        /// <summary>
        /// Returns dictionary with settings the specified type from configuration.
        /// </summary>
        /// <typeparam name="T">Setting class type</typeparam>
        /// <param name="configuration">Configuration provider</param>
        public static Dictionary<string, T>? GetDictionarySettings<T>(this IConfiguration configuration)
            where T : class
        {
            string settingsName = typeof(T).Name;
            Dictionary<string, T>? settings = configuration.GetSection(settingsName).Get<Dictionary<string, T>?>();
            _logger.Information("Reading settings {SettingsName}: {@Settings}", settingsName, settings);
            return settings;
        }

        /// Gets settings and validate it
        /// <typeparam name="TSetting">Settings type</typeparam>
        /// <typeparam name="TValidator">Validator type</typeparam>
        /// <param name="configuration">Configuration provider</param>
        /// <exception cref="ConfigurationMissingException">Thrown if getting of settings instance was unsuccessful.</exception>
        /// <exception cref="ConfigurationValidationException">Thrown if validation of settings instance was unsuccessful</exception>
        /// <returns>Returns the instance of settings type if finding and validating the settings was successful.</returns>
        public static TSetting GetSettingsAndValidate<TSetting, TValidator>(this IConfiguration configuration)
            where TValidator : AbstractValidator<TSetting>, new()
        {
            var settings = configuration.GetSettings<TSetting>();
            ThrowExceptionIfNull(settings);
            var validationResult = new TValidator().Validate(settings!);

            if (!validationResult.IsValid)
            {
                var settingsName = typeof(TSetting).Name;
                _logger.Error("Error occurred while validating settings for {SettingsName}", settingsName);
                Debug.Assert(false, $"Error while validating settings for {settingsName}");
                Dictionary<string, string> validationErrors = validationResult.Errors.ToDictionary(_ => _.PropertyName + _.ErrorCode, _ => _.ErrorMessage);
                throw new ConfigurationValidationException(settingsName, validationErrors);
            }

            _logger.Information("The following settings have been used for the {@SettingsName}: {@Settings}", typeof(TSetting).Name, settings);

            return settings!;
        }
        /// <summary>
        /// Throw ConfigurationMissingException is settings is null.
        /// </summary>
        /// <typeparam name="T">Settings type param</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="sectionName">Setting section name</param>
        /// <exception cref="ConfigurationMissingException">Thrown if settings is null.</exception>
        private static void ThrowExceptionIfNull<T>(T? settings, string? sectionName = null)
        {
            if (settings is not null)
            { return; }

            var settingsName = sectionName ?? typeof(T).Name;
            _logger.Error("Error occurred while getting settings for {SettingsName}, because the specified settings couldn't be found", settingsName);
            Debug.Assert(false, $"Error while getting settings for {settingsName}, because the specified settings couldn't be found");
            throw new ConfigurationMissingException(settingsName);
        }

        /// <summary>
        /// Searches settings section by name of settingsType and registers the section into IoC.
        /// </summary>
        /// <param name="services">IoC service collection</param>
        /// <param name="configuration">Configuration provider</param>
        /// <param name="settingsType">Setting class type</param>
        /// <param name="sectionName">Setting section name</param>
        private static IServiceCollection ConfigureSettings(this IServiceCollection services, IConfiguration configuration, Type settingsType, string? sectionName = null)
        {
            var method = typeof(OptionsConfigurationServiceCollectionExtensions).GetMethod(
                nameof(OptionsConfigurationServiceCollectionExtensions.Configure),
                new[] { typeof(IServiceCollection), typeof(IConfiguration) });

            IConfigurationSection section = string.IsNullOrWhiteSpace(sectionName) ? configuration.GetSection(settingsType.Name) : configuration.GetSection(sectionName);
            // We sure, that the method is not null
            method!.MakeGenericMethod(settingsType).Invoke(null, new object[] { services, section });

            return services;
        }

        /// <summary>
        /// Returns settings of the specified type, based on matching name of settings class type and section name.
        /// Class name of settings has to be the same as configuration section name
        /// </summary>
        /// <typeparam name="T">Settings class type</typeparam>
        /// <param name="configuration">Configuration provider</param>
        /// <returns>The instance of settings type if successful, <c>null</c> if the specified section couldn't be found</returns>
        public static T? GetSettings<T>(this IConfiguration configuration) =>
            (T?)GetSettings(configuration, typeof(T));

        /// <summary>
        /// Returns settings the specified type, based on matching name of settings class type and section name.
        /// Class name of settings has to be the same as configuration section name
        /// </summary>
        /// <param name="configuration">Configuration provider</param>
        /// <param name="settingsType">Settings class type</param>
        /// <returns>The instance of settings type if successful, <c>null</c> if the specified section couldn't be found</returns>
        private static object? GetSettings(this IConfiguration configuration, Type settingsType)
        {
            if (settingsType == null)
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(settingsType));
            }

            return configuration.GetSettingsByName(settingsType.Name, settingsType);
        }

        /// <summary>
        /// Returns settings of the specified type by configuration section name 
        /// </summary>
        /// <param name="configuration">Configuration provider</param>
        /// <param name="settingsName">Section name in the configuration</param>
        /// <param name="settingsType">Settings class type</param>
        /// <returns>The instance of settings type if successful, <c>null</c> if the specified section couldn't be found</returns>
        private static object? GetSettingsByName(this IConfiguration configuration, string settingsName, Type settingsType)
        {
            if (string.IsNullOrEmpty(settingsName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(settingsName));
            }

            var settings = configuration.GetSection(settingsName).Get(settingsType);
            _logger.Information("Reading configuration {@SettingsName} settings: {Settings}", settingsName, settings);

            return settings;
        }

        /// <summary>
        /// Base builder e.g. for tests, migrations
        /// </summary>
        public static IConfigurationBuilder GetConfigurationBuilder()
        {
            var configurationBuilder = new ConfigurationBuilder();
            return configurationBuilder.SetupConfigurationBuilder();
        }

        /// <summary>
        /// Base builder e.g. for tests
        /// </summary>
        public static IConfigurationBuilder SetupConfigurationBuilder(this ConfigurationBuilder configurationBuilder)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            configurationBuilder
                .AddJsonFile("appsettings.common.json", true, true)
                .AddJsonFile($"appsettings.common.{environmentName}.json", true, true)
                .AddJsonFile("appsettings.common.logging.json", true, true)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                .AddJsonFile("appsettings.logging.json", true, true)
                .AddEnvironmentVariables();

            return configurationBuilder;
        }

        public static IConfigurationBuilder SetupConfigurationBuilderForTests(this ConfigurationBuilder configurationBuilder, string? outputPath = null)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            configurationBuilder
                .SetBasePath(!string.IsNullOrEmpty(outputPath) ? outputPath : Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.common.json", true, true)
                .AddJsonFile($"appsettings.common.{environmentName}.json", true, true)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                .AddEnvironmentVariables();

            return configurationBuilder;
        }
    }
}
