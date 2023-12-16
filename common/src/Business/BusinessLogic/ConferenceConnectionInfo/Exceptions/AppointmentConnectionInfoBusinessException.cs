using System;
using System.Collections;
using System.Runtime.Serialization;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;

namespace Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Exceptions
{
    /// <summary>
    /// Abstract exception type for all types of business exceptions for video conferences
    /// </summary>
    [Serializable]
    public abstract class AppointmentConnectionInfoBusinessException : BusinessException
    {
        public new enum ErrorType
        {
            TooEarlyToRequestConnectionInfo,
            TooLateToRequestConnectionInfo,
            ConnectionInfoRequestedByWrongUser,
        }

        /// <inheritdoc />
        protected AppointmentConnectionInfoBusinessException(Guid appointmentId, string message, Enum code, Exception? innerException = null) : base(message, code, innerException)
        {
            AppointmentId = appointmentId;
        }

        /// <inheritdoc />
        protected AppointmentConnectionInfoBusinessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected AppointmentConnectionInfoBusinessException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }

        public Guid AppointmentId
        {
            get => GetProperty<Guid>();
            set => SetProperty(value);
        }
    }
}
