using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business
{
    /// <summary>
    /// Represent entity not found exception
    /// </summary>
    [Serializable]
    public abstract class EntityNotFoundException : BusinessException
    {
        /// <inheritdoc />
        protected EntityNotFoundException(Type type, string message, Enum code, Exception? innerException = null) : base(message, code, innerException)
        {
            Type = type.FullName;
        }

        /// <inheritdoc />
        protected EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected EntityNotFoundException(string message, IDictionary data, Exception? innerException = null)
            : base(message, data, innerException)
        {
        }

        public string? Type
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }
    }
}
