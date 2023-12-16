using System;
using System.Collections;
using System.Runtime.Serialization;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common.Exceptions
{
    /// <summary>
    /// Represent entity not found by innerId exception
    /// </summary>
    [Serializable]
    public class EntityNotFoundByInnerIdException : EntityNotFoundException
    {
        public EntityNotFoundByInnerIdException(Type type, string innerId, Exception? innerException = null)
            : base(type, $"Entity of type '{type.FullName}' and InnerId '{innerId}' can not be found.", ErrorType.EntityNotFound, innerException)
        {
            InnerId = innerId;
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        protected EntityNotFoundByInnerIdException(string message, IDictionary data, Exception innerException) : base(message, data, innerException)
        {
        }

        /// <inheritdoc />
        protected EntityNotFoundByInnerIdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string? InnerId
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }
    }
}
