using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Telemedicine.Common.Contracts.GlobalContracts.Exceptions
{
    /// <summary>
    /// Represent data access layer exception
    /// </summary>
    [Serializable]
    public class DalException : ServiceLayerException
    {
        public enum ErrorType
        {
            EmptyCode
        }

        /// <inheritdoc />
        public DalException(string message, Enum code, Exception? innerException = null)
            : base(message, code, innerException)
        {
        }


        /// <inheritdoc />
        public DalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected DalException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }
    }
}
