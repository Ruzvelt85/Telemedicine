using System;
using Telemedicine.Common.Infrastructure.SftpClient.Exceptions;
using Telemedicine.Common.Infrastructure.SftpClient.Extensions;
using Renci.SshNet;
using Serilog;
using ExternalSftpClient = Renci.SshNet.SftpClient;

namespace Telemedicine.Common.Infrastructure.SftpClient
{
    ///<inheritdoc cref="IExternalSftpClientFactory"/>
    internal class ExternalSftpClientFactory : IExternalSftpClientFactory
    {
        private readonly ILogger _logger = Log.ForContext<ExternalSftpClientFactory>();

        ///<inheritdoc cref="IExternalSftpClientFactory.Create"/>
        public ExternalSftpClient Create(SftpClientSettings sftpClientSettings)
        {
            try
            {
                _logger.Debug("Reading private key.");
                using var stream = sftpClientSettings.PrivateKey.GetStream();
                var privateKey = new PrivateKeyFile(stream, sftpClientSettings.PassPhrase);
                var privateKeyFile = new[] { privateKey };
                var privateKeyAuthenticationMethod = new PrivateKeyAuthenticationMethod(sftpClientSettings.Username, privateKeyFile);
                var connectionInfo = new ConnectionInfo(sftpClientSettings.Host, sftpClientSettings.Port, sftpClientSettings.Username, privateKeyAuthenticationMethod);

                _logger.Debug("Creating external SFTP client.");
                var sftpClient = new ExternalSftpClient(connectionInfo);

                if (sftpClientSettings.KeepAliveIntervalInSeconds != -1)
                {
                    sftpClient.KeepAliveInterval = TimeSpan.FromSeconds(sftpClientSettings.KeepAliveIntervalInSeconds);
                }

                return sftpClient;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An exception occurred while initializing SFTP client. Message: {ErrorMessage}", ex.Message);
                throw new InitializeSftpClientException(ex);
            }
        }
    }
}
