using System;

namespace Telemedicine.Common.Infrastructure.SftpClient.Exceptions
{
    [Serializable]
    public class OpenReadSftpClientException : SftpClientException
    {
        public OpenReadSftpClientException(Exception innerException)
            : base("Can't open an existing file for reading.", innerException)
        {
        }
    }
}
