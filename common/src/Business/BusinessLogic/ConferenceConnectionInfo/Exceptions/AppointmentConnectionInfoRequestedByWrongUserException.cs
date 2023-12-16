using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Exceptions
{
    /// <summary>
    /// Represents exception that should be thrown if connection info for appointment was requested by user that wasn't an attendee of appointment
    /// </summary>
    [Serializable]
    public class AppointmentConnectionInfoRequestedByWrongUserException : AppointmentConnectionInfoBusinessException
    {
        /// <inheritdoc />
        public AppointmentConnectionInfoRequestedByWrongUserException(Guid appointmentId, Guid userId, Exception? innerException = null)
            : base(appointmentId, $"Connection info for appointment with ID='{appointmentId}' is requested by user with ID='{userId}' that isn't an attendee.",
                ErrorType.ConnectionInfoRequestedByWrongUser, innerException)
        {
            UserId = userId;
        }

        /// <inheritdoc />
        public AppointmentConnectionInfoRequestedByWrongUserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected AppointmentConnectionInfoRequestedByWrongUserException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }

        public Guid UserId
        {
            get => GetProperty<Guid>();
            set => SetProperty(value);
        }
    }
}
