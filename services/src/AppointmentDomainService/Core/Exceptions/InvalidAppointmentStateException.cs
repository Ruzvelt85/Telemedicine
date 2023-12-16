using System;
using System.Collections;
using System.Runtime.Serialization;
using Telemedicine.Common.Infrastructure.EntityBase;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;

namespace Telemedicine.Services.AppointmentDomainService.Core.Exceptions
{
    [Serializable]
    public class InvalidAppointmentStateException : DomainException
    {
        public new enum ErrorType
        {
            Empty = 0
        }

        public InvalidAppointmentStateException(string message, Guid appointmentId, AppointmentState appointmentState, Exception? innerException = null)
    : base(message, ErrorType.Empty, innerException)
        {
            AppointmentId = appointmentId;
            AppointmentState = appointmentState;
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected InvalidAppointmentStateException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }

        /// <inheritdoc />
        protected InvalidAppointmentStateException(SerializationInfo info, StreamingContext context)
         : base(info, context)
        {

        }

        public Guid AppointmentId
        {
            get => GetProperty<Guid>();
            set => SetProperty(value);
        }

        public AppointmentState AppointmentState
        {
            get => GetProperty<AppointmentState>();
            set => SetProperty(value);
        }
    }
}
