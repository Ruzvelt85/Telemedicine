using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Telemedicine.Common.Contracts.GlobalContracts.Exceptions
{
    /// <summary>
    /// Represent validation exception
    /// </summary>
    [Serializable]
    public class ValidationException : ServiceLayerException
    {
        public enum ErrorType
        {
            EmptyCode
        }

        /// <inheritdoc />
        public ValidationException(string message, Enum code, Exception? innerException = null) : base(message, code, innerException)
        {
        }

        /// <inheritdoc />
        public ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected ValidationException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }
    }
}
