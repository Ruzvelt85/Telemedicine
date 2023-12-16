using System;
using System.Collections;
using System.Runtime.Serialization;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;

namespace Telemedicine.Common.Infrastructure.EntityBase
{
    [Serializable]
    public abstract class DomainException : BusinessException
    {
        protected DomainException(string message, Enum code, Exception? innerException = null) : base(message, code, innerException)
        { }

        protected DomainException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected DomainException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }
    }
}
