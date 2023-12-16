using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceCommandService.Exceptions
{
    public class CreateConferenceException : VidyoIntegrationException
    {
        /// <inheritdoc />
        public CreateConferenceException(string message, Exception? innerException = null)
            : base($"Operation 'Create Conference' failed with error: {message}", ErrorType.CreatingConferenceFailed, innerException)
        {
        }

        /// <inheritdoc />
        public CreateConferenceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected CreateConferenceException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }
    }
}
