using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Unexpected
{
    /// <summary>
    /// Represent unexpected exception
    /// </summary>
    [Serializable]
    public class UnexpectedException : ServiceLayerException
    {
        public enum ErrorType
        {
            UnexpectedException = 0,
            ConfigurationException = 1,
            UnexpectedHttpIntegration = 2
        }

        /// <inheritdoc />
        public UnexpectedException(string message, Enum code, Exception? innerException = null)
            : base(message, code, innerException)
        {
        }

        /// <inheritdoc />
        public UnexpectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected UnexpectedException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }
    }
}
