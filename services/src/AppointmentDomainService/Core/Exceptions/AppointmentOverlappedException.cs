using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Telemedicine.Common.Infrastructure.EntityBase;

namespace Telemedicine.Services.AppointmentDomainService.Core.Exceptions
{
    [Serializable]
    public class AppointmentOverlappedException : DomainException
    {
        public new enum ErrorType
        {
            Empty = 0
        }

        public AppointmentOverlappedException(Guid[] overlappedAppointments, Exception? innerException = null)
            : base($"There are overlapped appointments ('{string.Join("', '", overlappedAppointments)}').", ErrorType.Empty, innerException)
        {
            OverlappedAppointments = overlappedAppointments;
        }

        public AppointmentOverlappedException(Guid appointmentId, Guid[] overlappedAppointments, Exception? innerException = null)
            : base($"Appointment with id '{appointmentId}' has already overlapped with next appointments ('{string.Join("', '", overlappedAppointments)}').", ErrorType.Empty, innerException)
        {
            OverlappedAppointments = overlappedAppointments;
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected AppointmentOverlappedException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }

        /// <inheritdoc />
        protected AppointmentOverlappedException(SerializationInfo info, StreamingContext context)
         : base(info, context)
        {

        }

        public IEnumerable<Guid> OverlappedAppointments
        {
            get => GetProperty<IEnumerable<Guid>>() ?? Array.Empty<Guid>();
            set => SetProperty(value);
        }
    }
}
