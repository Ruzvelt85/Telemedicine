namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.Settings
{
    /// <summary>
    /// Do not use directly. It use only via builder <see cref="ISaturationMeasurementSettingsBuilder" />
    /// </summary>
    public class SaturationMeasurementSettings
    {
        /// <summary>
        /// The maximum number of saturation raw measurements that will be accepted by the validator
        /// This setting should be consistent with a similar setting in the BFF (it must be equal or great than BFF limit)
        /// </summary>
        public int MaxRawItemsValidationCountLimit { get; set; } = 100;
    }
}
