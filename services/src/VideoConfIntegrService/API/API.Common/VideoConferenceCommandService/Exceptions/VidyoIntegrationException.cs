using System;
using System.Collections;
using System.Runtime.Serialization;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;

namespace Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceCommandService.Exceptions
{
    public abstract class VidyoIntegrationException : BusinessException
    {
        public new enum ErrorType
        {
            CreatingConferenceFailed,
            SettingPinCodeFailed
        }

        /// <inheritdoc />
        protected VidyoIntegrationException(string message, Enum code, Exception? innerException = null) : base(message, code, innerException)
        {
        }


        /// <inheritdoc />
        protected VidyoIntegrationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected VidyoIntegrationException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }
    }
}
