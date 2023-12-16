namespace Telemedicine.Services.VideoConfIntegrService.WebAPI.Services
{
    /// <summary>
    /// Setting for configuration creating conferences
    /// </summary>
    public record ConferenceSettings
    {
        /// <summary>
        /// Tenant's extension
        /// </summary>
        public string? ExtensionPrefix { get; init; }

        /// <summary>
        /// To set PIN code for room
        /// </summary>
        public bool IsSetPinCode { get; init; }

        /// <summary>
        /// Digit quantity for PIN code (should be from 6 to 12)
        /// </summary>
        public int PinCodeFormat { get; init; } = 10;
    }
}
