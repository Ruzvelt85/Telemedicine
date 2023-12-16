using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business
{
    /// <summary>
    /// Represent entity already deleted exception
    /// </summary>
    [Serializable]
    public class EntityAlreadyDeletedException : BusinessException
    {
        /// <inheritdoc />
        public EntityAlreadyDeletedException(Type type, Guid id, Exception? innerException = null)
            : base($"Entity with type '{type.FullName}' and id '{id}' has already been deleted.", ErrorType.EntityAlreadyDeleted, innerException)
        {
            Type = type.FullName;
            Id = id;
        }

        /// <inheritdoc />
        public EntityAlreadyDeletedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        public EntityAlreadyDeletedException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }

        public string? Type
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public Guid Id
        {
            get => GetProperty<Guid>();
            set => SetProperty(value);
        }
    }
}
