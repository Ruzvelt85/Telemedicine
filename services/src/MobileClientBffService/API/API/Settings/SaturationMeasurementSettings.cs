namespace Telemedicine.Services.MobileClientBffService.API.Settings
{
    /// <summary>
    /// Do not use directly. It use only via builder <see cref="ISaturationMeasurementSettingsBuilder" />
    /// </summary>
    public class SaturationMeasurementSettings
    {
        /// <summary>
        /// The maximum number of saturation raw measurements that will be accepted by the validator
        /// This setting should be consistent with a similar setting in the Health Measurement Domain Service (it must be equal or less than domain service limit)
        /// </summary>
        public int MaxRawItemsValidationCountLimit { get; set; } = 100;

        /// <summary>
        /// The maximum number of saturation raw measurements that will be pass through BFF (other will be discarded) 
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public int MaxRawItemsToPassToMeasurementDSCountLimit { get; set; } = 30;
    }
}