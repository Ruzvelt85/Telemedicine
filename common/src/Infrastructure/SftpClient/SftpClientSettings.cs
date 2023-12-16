using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase;

namespace Telemedicine.Common.Infrastructure.SftpClient
{
    public record SftpClientSettings
    {
        internal static int DefaultPort = 22;

        internal static int KeepAliveDefaultInterval = -1;

        public string Host { get; init; } = string.Empty;

        public int Port { get; init; } = DefaultPort;

        /// <summary>
        /// Specify negative one (-1) to disable the keep-alive. This is the default value.
        /// </summary>
        public int KeepAliveIntervalInSeconds { get; init; } = KeepAliveDefaultInterval;

        [DataLogMasked]
        public string Username { get; init; } = string.Empty;

        [DataLogMasked]
        public string PrivateKey { get; init; } = string.Empty;

        [DataLogMasked]
        public string PassPhrase { get; init; } = string.Empty;
    }
}
