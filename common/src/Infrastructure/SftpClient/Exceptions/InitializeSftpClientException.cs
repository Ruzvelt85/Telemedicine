using System;

namespace Telemedicine.Common.Infrastructure.SftpClient.Exceptions
{
    [Serializable]
    public class InitializeSftpClientException : SftpClientException
    {
        public InitializeSftpClientException(Exception innerException)
            : base("Cannot initialize SFTP client.", innerException)
        {
        }
    }
}
