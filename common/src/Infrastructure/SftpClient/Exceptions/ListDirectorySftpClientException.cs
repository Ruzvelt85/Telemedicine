using System;

namespace Telemedicine.Common.Infrastructure.SftpClient.Exceptions
{
    [Serializable]
    public class ListDirectorySftpClientException : SftpClientException
    {
        public ListDirectorySftpClientException(Exception innerException)
            : base("Can't retrieve list of files in remote directory.", innerException)
        {
        }
    }
}
