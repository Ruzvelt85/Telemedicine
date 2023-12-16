using System;
using System.Collections;
using System.Collections.Generic;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Exceptions
{
    public class AppointmentOverlappedException : AppointmentBusinessException
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

        /// <inheritdoc />
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

        public IEnumerable<Guid> OverlappedAppointments
        {
            get => GetProperty<IEnumerable<Guid>>() ?? Array.Empty<Guid>();
            set => SetProperty(value);
        }

    }
}
