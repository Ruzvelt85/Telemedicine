using System;
using System.Collections;
using System.Net;
using System.Runtime.Serialization;

namespace Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Unexpected
{
    /// <summary>
    /// Represents unexpected exception with services integration
    /// </summary>
    [Serializable]
    public class UnexpectedHttpIntegrationException : UnexpectedException
    {
        public UnexpectedHttpIntegrationException(string? uri, string? action, HttpStatusCode httpStatusCode, Exception? innerException = null)
            : base($"Unexpected exception was thrown while getting response from Uri={uri ?? string.Empty}.", ErrorType.UnexpectedHttpIntegration, innerException)
        {
            Uri = uri;
            Action = action;
            HttpStatusCode = httpStatusCode;
        }

        /// <inheritdoc />
        public UnexpectedHttpIntegrationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected UnexpectedHttpIntegrationException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }

        public string? Uri
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public string? Action
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public HttpStatusCode HttpStatusCode
        {
            get => GetProperty<HttpStatusCode>();
            set => SetProperty(value);
        }
    }
}
