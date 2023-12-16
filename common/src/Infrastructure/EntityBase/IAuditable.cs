using System;
using JetBrains.Annotations;

namespace Telemedicine.Common.Infrastructure.EntityBase
{
    /// <summary>
    /// Interface for use with entities that need to be audited
    /// </summary>
    [PublicAPI]
    public interface IAuditable
    {
        /// <summary>
        /// Date and time when the entity record was created
        /// </summary>
        DateTime CreatedOn { get; }

        /// <summary>
        /// Date and time of the last update of the entity record
        /// </summary>
        DateTime UpdatedOn { get; }
    }
}
