using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Exceptions
{
    /// <summary>
    /// Represents exception that should be thrown if connection info for appointment was requested too late
    /// </summary>
    [Serializable]
    public class AppointmentConnectionInfoRequestedTooLateException : AppointmentConnectionInfoBusinessException
    {
        /// <inheritdoc />
        public AppointmentConnectionInfoRequestedTooLateException(Guid appointmentId, DateTime startDate, DateTime endDate, Exception? innerException = null)
            : base(appointmentId, $"Connection info for appointment with ID='{appointmentId}', StartDate='{startDate:G}' and EndDate='{endDate:G}' is requested too late.",
                ErrorType.TooLateToRequestConnectionInfo, innerException)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <inheritdoc />
        public AppointmentConnectionInfoRequestedTooLateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected AppointmentConnectionInfoRequestedTooLateException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }

        public DateTime StartDate
        {
            get => GetProperty<DateTime>();
            set => SetProperty(value);
        }

        public DateTime EndDate
        {
            get => GetProperty<DateTime>();
            set => SetProperty(value);
        }
    }
}
