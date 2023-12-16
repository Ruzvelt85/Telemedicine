using System;
using System.Collections;
using System.Runtime.Serialization;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common.Exceptions
{
    public abstract class HealthMeasurementDomainException : BusinessException
    {
        public enum DomainErrorType
        {
            MoodAlreadyCreatedToday
        }

        protected HealthMeasurementDomainException(string message, Enum code, Exception? innerException = null)
            : base(message, code, innerException)
        {
        }

        protected HealthMeasurementDomainException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        [Obsolete("Don't use this constructor. The constructor is used to automatically restore the exception.", true)]
        protected HealthMeasurementDomainException(string message, IDictionary data, Exception? innerException = null)
            : base(message, data, innerException)
        {
        }
    }
}
