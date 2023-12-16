using System;
using System.Runtime.Serialization;

namespace Telemedicine.Common.Infrastructure.VidyoClient.Exceptions
{
    [Serializable]
    public class CreateConferenceException : VidyoClientException
    {
        public CreateConferenceException(string message, Exception? innerException = null)
            : base($"Operation 'Create Conference' failed with error: {message}", innerException)
        {
        }

        protected CreateConferenceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
