using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Exceptions
{
    /// <summary>
    /// Represents exception that should be thrown if connection info for appointment was requested too early
    /// </summary>
    [Serializable]
    public class AppointmentConnectionInfoRequestedTooEarlyException : AppointmentConnectionInfoBusinessException
    {
        /// <inheritdoc />
        public AppointmentConnectionInfoRequestedTooEarlyException(Guid appointmentId, DateTime startDate, Exception? innerException = null)
            : base(appointmentId, $"Connection info for appointment with ID='{appointmentId}' and StartDate='{startDate:G}' is requested too early.",
                ErrorType.TooEarlyToRequestConnectionInfo, innerException)
        {
            StartDate = startDate;
        }

        /// <inheritdoc />
        public AppointmentConnectionInfoRequestedTooEarlyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected AppointmentConnectionInfoRequestedTooEarlyException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }

        public DateTime StartDate
        {
            get => GetProperty<DateTime>();
            set => SetProperty(value);
        }
    }
}
