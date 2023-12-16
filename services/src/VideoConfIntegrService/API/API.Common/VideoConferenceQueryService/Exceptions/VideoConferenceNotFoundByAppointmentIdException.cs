using System;
using System.Collections;
using System.Runtime.Serialization;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;

namespace Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceQueryService.Exceptions
{
    /// <summary>
    /// Represents video conference not found exception
    /// </summary>
    [Serializable]
    public class VideoConferenceNotFoundByAppointmentIdException : EntityNotFoundException
    {
        public VideoConferenceNotFoundByAppointmentIdException(Type type, Guid appointmentId, Exception? innerException = null)
            : base(type, $"Video conference with AppointmentId='{appointmentId}' cannot be found.", ErrorType.EntityNotFound, innerException)
        {
            AppointmentId = appointmentId;
        }

        /// <inheritdoc />
        public VideoConferenceNotFoundByAppointmentIdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected VideoConferenceNotFoundByAppointmentIdException(string message, IDictionary data, Exception innerException) : base(message, data, innerException)
        {
        }

        public Guid AppointmentId
        {
            get => GetProperty<Guid>();
            set => SetProperty(value);
        }
    }
}
