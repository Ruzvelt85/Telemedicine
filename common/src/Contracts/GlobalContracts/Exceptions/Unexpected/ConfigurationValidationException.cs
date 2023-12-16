using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Unexpected
{
    /// <summary>
    /// Represent configuration validation exception
    /// </summary>
    [Serializable]
    public class ConfigurationValidationException : ConfigurationException
    {
        /// <summary>
        /// Dictionary that contains validation errors where Key is property name and Value is error message
        /// </summary>
        [PublicAPI]
        public Dictionary<string, string> ValidationErrors
        {
            get => GetProperty<Dictionary<string, string>>()!;
            set => SetProperty(value);
        }

        /// <param name="configurationName">Configuration name</param>
        /// <param name="validationErrors">Dictionary that contains validation errors where Key is property name and Value is error message</param>
        /// <param name="innerException">Inner exception</param>
        public ConfigurationValidationException(string configurationName, Dictionary<string, string>? validationErrors = null, Exception? innerException = null)
            : base($"Configuration validation exception occurred for settings '{configurationName}'", configurationName, ErrorType.ConfigurationValidationException, innerException)
        {
            ConfigurationName = configurationName;
            ValidationErrors = validationErrors ?? new Dictionary<string, string>();
        }

        /// <inheritdoc />
        public ConfigurationValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected ConfigurationValidationException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }
    }
}
