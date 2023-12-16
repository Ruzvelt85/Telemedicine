using System;
using System.Collections;
using System.Runtime.Serialization;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common.Exceptions
{
    [Serializable]
    public abstract class HealthCenterStructureDomainBusinessException : BusinessException
    {
        /// <inheritdoc />
        protected HealthCenterStructureDomainBusinessException(string message, ErrorType code, Exception? innerException = null)
            : base(message, code, innerException)
        {
        }

        /// <inheritdoc />
        protected HealthCenterStructureDomainBusinessException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected HealthCenterStructureDomainBusinessException(string message, IDictionary data, Exception? innerException = null)
            : base(message, data, innerException)
        {
        }
    }
}
