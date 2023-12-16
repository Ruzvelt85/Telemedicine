using System;
using System.Collections;
using System.Runtime.Serialization;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Unexpected;

namespace Telemedicine.Common.Contracts.GlobalContracts.Exceptions
{
    public class ConfigurationMissingException : ConfigurationException
    {
        /// <param name="configurationName">Configuration name</param>
        /// <param name="innerException">Inner exception</param>
        public ConfigurationMissingException(string configurationName, Exception? innerException = null)
            : base($"Configuration {configurationName} is missing", configurationName, ErrorType.ConfigurationMissingException, innerException)
        {
            ConfigurationName = configurationName;
        }

        /// <inheritdoc />
        public ConfigurationMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected ConfigurationMissingException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }
    }
}
