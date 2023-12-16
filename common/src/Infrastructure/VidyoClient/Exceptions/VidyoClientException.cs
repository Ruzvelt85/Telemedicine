using System;
using System.Runtime.Serialization;

namespace Telemedicine.Common.Infrastructure.VidyoClient.Exceptions
{
    [Serializable]
    public abstract class VidyoClientException : Exception
    {
        protected VidyoClientException()
        {
        }

        protected VidyoClientException(string message) : base(message)
        {
        }

        protected VidyoClientException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }

        protected VidyoClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
