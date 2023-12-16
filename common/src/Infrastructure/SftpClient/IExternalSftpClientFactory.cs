using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Unexpected;
using Telemedicine.Common.Infrastructure.SftpClient.Exceptions;
using ExternalSftpClient = Renci.SshNet.SftpClient;

namespace Telemedicine.Common.Infrastructure.SftpClient
{
    /// <summary>
    /// Factory for <see cref="ExternalSftpClient"/>
    /// </summary>
    public interface IExternalSftpClientFactory
    {
        /// <summary>
        /// Creates a configured object of <see cref="ExternalSftpClient"/>
        /// </summary>
        /// <param name="sftpClientSettings">Settings that are used for creating an object of <see cref="ExternalSftpClient"/></param>
        /// <returns>Configured <see cref="ExternalSftpClient"/></returns>
        /// <exception cref="ConfigurationValidationException">Thrown when the provided settings is not valid.</exception>
        /// <exception cref="InitializeSftpClientException">Thrown when the SFTP client failed to initialize.</exception>
        ExternalSftpClient Create(SftpClientSettings sftpClientSettings);
    }
}
