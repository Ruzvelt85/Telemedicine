using System;
using System.Collections;
using System.Runtime.Serialization;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Exceptions
{
    [Serializable]
    public abstract class AppointmentBusinessException : BusinessException
    {
        /// <inheritdoc />
        protected AppointmentBusinessException(string message, Enum code, Exception? innerException = null) : base(message, code, innerException)
        {
        }

        /// <inheritdoc />
        protected AppointmentBusinessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected AppointmentBusinessException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }
    }
}
