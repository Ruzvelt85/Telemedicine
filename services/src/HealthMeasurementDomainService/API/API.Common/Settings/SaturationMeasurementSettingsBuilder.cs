using System;
using Microsoft.Extensions.Options;
using Serilog;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.Settings
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

        private const string SettingsHaveIncorrectValuesLogMsg = "The specified value {SettingValue} for {SettingName} is not correct ({MaxRawItemsValidationCountLimit} <= 0). The default value is used instead.";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public SaturationMeasurementSettings Build()
        {
            try
            {
                SaturationMeasurementSettings settings = _saturationMeasurementSettingsSnapshot.Value;

                if (settings.MaxRawItemsValidationCountLimit <= 0)
                {
                    _logger.Warning(SettingsHaveIncorrectValuesLogMsg, settings.MaxRawItemsValidationCountLimit, nameof(settings.MaxRawItemsValidationCountLimit), nameof(SaturationMeasurementSettings.MaxRawItemsValidationCountLimit));
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
