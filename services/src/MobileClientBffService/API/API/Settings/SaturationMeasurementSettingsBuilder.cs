using Microsoft.Extensions.Options;
using Serilog;
using System;

namespace Telemedicine.Services.MobileClientBffService.API.Settings
{
    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public class SaturationMeasurementSettingsBuilder : ISaturationMeasurementSettingsBuilder
    {
        private readonly ILogger _logger = Log.ForContext<SaturationMeasurementSettingsBuilder>();
        private readonly IOptionsSnapshot<SaturationMeasurementSettings> _saturationMeasurementSettingsSnapshot;

        public SaturationMeasurementSettingsBuilder(IOptionsSnapshot<SaturationMeasurementSettings> saturationMeasurementSettingsSnapshot)
        {
            _saturationMeasurementSettingsSnapshot = saturationMeasurementSettingsSnapshot;
        }

        private const string SettingsHaveIncorrectValuesLogMsg = "The specified values maxRawItemsValidationCountLimit = '{MaxRawItemsValidationCountLimit}' maxRawItemsToPassToMeasurementDSCountLimit = '{MaxRawItemsToPassToMeasurementDSCountLimit}' for {SettingName} are not " +
                                                                 "correct (MaxRawItemsToPassToMeasurementDSCountLimit <= 0 || MaxRawItemsValidationCountLimit < MaxRawItemsToPassToMeasurementDSCountLimit). The default values, will be used instead.";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public SaturationMeasurementSettings Build()
        {
            try
            {
                SaturationMeasurementSettings settings = _saturationMeasurementSettingsSnapshot.Value;

                int maxRawItemsValidationCountLimit = settings.MaxRawItemsValidationCountLimit;
                int maxRawItemsToPassToMeasurementDsCountLimit = settings.MaxRawItemsToPassToMeasurementDSCountLimit;

                if (maxRawItemsToPassToMeasurementDsCountLimit <= 0 || maxRawItemsValidationCountLimit < maxRawItemsToPassToMeasurementDsCountLimit)
                {
                    _logger.Warning(SettingsHaveIncorrectValuesLogMsg, nameof(SaturationMeasurementSettings), maxRawItemsValidationCountLimit, maxRawItemsToPassToMeasurementDsCountLimit);
                    return new SaturationMeasurementSettings();
                }
                return settings;
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "An unexpected exception occurred while reading {SettingName}", nameof(SaturationMeasurementSettings));
            }
            return new SaturationMeasurementSettings();
        }
    }
}