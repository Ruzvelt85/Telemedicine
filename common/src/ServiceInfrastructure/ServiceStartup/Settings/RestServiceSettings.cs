namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Settings
{
    /// <summary>
    /// Setting for configuration Refit-client of REST service
    /// </summary>
    public record RestServiceSettings
    {
        public RestServiceSettings()
        {
            HealthCheckSettings = new HealthCheckSettings();
        }

        /// <summary>
        /// Name of REST service
        /// </summary>
        public string? Name { get; init; }

        /// <summary>
        /// Full Url to REST service endpoint
        /// </summary>
        public string? Url { get; init; }

        /// <summary>
        /// Full qualified interface name of REST service
        /// </summary>
        public string? ServiceContract { get; init; }

        /// <summary>
        /// Settings for Health Check of REST service
        /// </summary>
        public HealthCheckSettings HealthCheckSettings { get; init; }
    }

    /// <summary>
    /// Setting for health check configuration
    /// </summary>
    public record HealthCheckSettings
    {
        /// <summary>
        /// Default URI for health check endpoint
        /// </summary>
        public const string DefaultHealthCheckUri = "/health";

        /// <summary>
        /// To check health of REST service
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Relative path to health check endpoint
        /// </summary>
        public string RelativePath { get; set; } = DefaultHealthCheckUri;
    }
}
