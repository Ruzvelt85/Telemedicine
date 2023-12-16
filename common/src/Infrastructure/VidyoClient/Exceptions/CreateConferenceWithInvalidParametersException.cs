using System;
using System.Runtime.Serialization;

namespace Telemedicine.Common.Infrastructure.VidyoClient.Exceptions
{
    [Serializable]
    public class CreateConferenceWithInvalidParametersException : CreateConferenceException
    {
        public CreateConferenceWithInvalidParametersException(string message, Exception? innerException = null)
            : base($"Operation 'Create Conference' failed with error due to invalid parameters: {message}", innerException)
        {
        }

        protected CreateConferenceWithInvalidParametersException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
