using System;
using System.Runtime.Serialization;

namespace Telemedicine.Common.Infrastructure.VidyoClient.Exceptions
{
    [Serializable]
    public class SetPinCodeException : VidyoClientException
    {
        public SetPinCodeException(string message, Exception? innerException = null)
            : base($"Operation 'Set PIN code' failed with error: {message}", innerException)
        {
        }

        protected SetPinCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
