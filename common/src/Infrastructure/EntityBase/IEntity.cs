using System;
using JetBrains.Annotations;

namespace Telemedicine.Common.Infrastructure.EntityBase
{
    /// <summary>
    /// Defines common properties for entity
    /// </summary>
    [PublicAPI]
    public interface IEntity
    {
        /// <summary>
        /// Id of the entity
        /// </summary>
        Guid Id { get; }
    }
}
