using System;
using System.Runtime.Serialization;

namespace Telemedicine.Common.Infrastructure.SftpClient.Exceptions
{
    [Serializable]
    public abstract class SftpClientException : Exception
    {
        protected SftpClientException()
        {
        }

        protected SftpClientException(string message) : base(message)
        {
        }

        protected SftpClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SftpClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
