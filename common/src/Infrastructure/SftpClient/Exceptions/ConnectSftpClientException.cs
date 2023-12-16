using System;

namespace Telemedicine.Common.Infrastructure.SftpClient.Exceptions
{
    [Serializable]
    public class ConnectSftpClientException : SftpClientException
    {
        public ConnectSftpClientException(Exception innerException)
            : base("Client cannot connect to the server via SFTP.", innerException)
        {
        }
    }
}
