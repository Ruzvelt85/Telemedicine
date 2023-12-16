using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.Exceptions
{
    public class MoodAlreadyCreatedTodayException : HealthMeasurementDomainException
    {
        private const string DefaultExceptionMessage = "Patient: {0} already created mood today.";
        private const string ExceptionMessageWithDateTime = "Patient: {0} already created mood today at {1}.";

        public MoodAlreadyCreatedTodayException(Guid patientId, Exception? innerException = null)
            : base(string.Format(DefaultExceptionMessage, patientId), DomainErrorType.MoodAlreadyCreatedToday, innerException)
        {
        }

        public MoodAlreadyCreatedTodayException(Guid patientId, DateTime dateTime, Exception? innerException = null)
            : base(string.Format(ExceptionMessageWithDateTime, patientId, dateTime), DomainErrorType.MoodAlreadyCreatedToday, innerException)
        {
        }

        /// <inheritdoc />
        public MoodAlreadyCreatedTodayException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected MoodAlreadyCreatedTodayException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }
    }
}
