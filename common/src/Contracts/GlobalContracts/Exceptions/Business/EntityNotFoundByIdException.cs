using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business
{
    /// <summary>
    /// Represent entity not found by id exception
    /// </summary>
    [Serializable]
    public class EntityNotFoundByIdException : EntityNotFoundException
    {
        public EntityNotFoundByIdException(Type type, Guid id, Exception? innerException = null)
            : base(type, $"Entity with type '{type.FullName}' and id '{id}' can not be found.", ErrorType.EntityNotFound, innerException)
        {
            Id = id;
        }

        /// <inheritdoc />
        public EntityNotFoundByIdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected EntityNotFoundByIdException(string message, IDictionary data, Exception innerException) : base(message, data, innerException)
        {
        }

        public Guid Id
        {
            get => GetProperty<Guid>();
            set => SetProperty(value);
        }
    }
}
