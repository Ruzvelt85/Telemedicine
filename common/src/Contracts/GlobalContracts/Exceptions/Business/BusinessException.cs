using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business
{
    /// <summary>
    /// Represent base business exception
    /// </summary>
    [Serializable]
    public abstract class BusinessException : ServiceLayerException
    {
        public enum ErrorType
        {
            EntityNotFound,
            EntityAlreadyDeleted,
        }

        /// <inheritdoc />
        protected BusinessException(string message, Enum code, Exception? innerException = null)
            : base(message, code, innerException)
        {
        }

        /// <inheritdoc />
        protected BusinessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected BusinessException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }
    }
}
