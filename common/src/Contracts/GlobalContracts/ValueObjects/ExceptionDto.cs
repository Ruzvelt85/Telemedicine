using System.Collections;

namespace Telemedicine.Common.Contracts.GlobalContracts.ValueObjects
{
    public record ExceptionDto
    {
        /// <summary>
        /// Exception code, string representation of the nested Enum of internal exception types
        /// </summary>
        public string? Code { get; init; }

        /// <summary>
        /// Exception message
        /// </summary>
        public string? Message { get; init; }

        /// <summary>
        /// Contains additional data describing the exception
        /// </summary>
        public IDictionary? Data { get; init; }

        /// <summary>
        /// Exception type
        /// </summary>
        public string? Type { get; init; }
    }
}
