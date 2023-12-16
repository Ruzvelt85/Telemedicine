namespace Telemedicine.Services.MobileClientBffService.API.Settings
{
    public interface ISaturationMeasurementSettingsBuilder
    {
        /// <summary>
        /// Validates <see cref="SaturationMeasurementSettings"/> and returns a valid setting
        /// </summary>
        /// <returns>Valid setting</returns>
        SaturationMeasurementSettings Build();
    }
}