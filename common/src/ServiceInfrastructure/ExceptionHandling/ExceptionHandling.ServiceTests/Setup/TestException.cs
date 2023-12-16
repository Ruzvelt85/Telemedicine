using System;
using System.Collections;
using System.Runtime.Serialization;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions;

namespace Telemedicine.Common.ServiceInfrastructure.ExceptionHandling.ServiceTests.Setup
{
    internal class TestException : ServiceLayerException
    {
        public enum ErrorCode
        {
            TestError
        }

        /// <inheritdoc />
        public TestException(string message, Enum code, Exception? innerException = null)
            : base(message, code, innerException)
        {
        }

        /// <inheritdoc />
        public TestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, false)]
        public TestException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }
    }
}
