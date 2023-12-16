using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Unexpected
{
    /// <summary>
    /// Represent configuration exception
    /// </summary>
    [Serializable]
    public class ConfigurationException : UnexpectedException
    {
        public new enum ErrorType
        {
            ConfigurationValidationException = 0,
            ConfigurationMissingException = 1,
        }

        public string? ConfigurationName
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        /// <inheritdoc />
        public ConfigurationException(string message, string configurationName, Exception? innerException = null)
            : this(message, configurationName, UnexpectedException.ErrorType.ConfigurationException, innerException)
        {
        }

        /// <inheritdoc />
        protected ConfigurationException(string message, string configurationName, Enum code, Exception? innerException = null)
            : base(message, code, innerException)
        {
            ConfigurationName = configurationName;
        }

        /// <inheritdoc />
        public ConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected ConfigurationException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }
    }
}
