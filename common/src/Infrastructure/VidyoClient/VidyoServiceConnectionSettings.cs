namespace Telemedicine.Common.Infrastructure.VidyoClient
{
    /// <summary>
    /// Setting for configuration WCF-client for Vidyo service
    /// </summary>
    public record VidyoServiceConnectionSettings
    {
        /// <summary>
        /// Full Url to WCF service endpoint
        /// </summary>
        public string Url { get; init; } = string.Empty;

        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; init; } = string.Empty;

        /// <summary>
        /// User password
        /// </summary>
        public string UserPassword { get; init; } = string.Empty;

        /// <summary>
        /// Timeout in ms
        /// </summary>
        public int Timeout { get; init; } = 10000;
    }
}
