using JetBrains.Annotations;

namespace Telemedicine.Common.Infrastructure.EntityBase
{
    /// <summary>
    /// Interface for use with objects that need soft deletion
    /// (soft deleted records has property <see cref="ILogicallyDeletable.IsDeleted"/> = true in the database)
    /// </summary>
    [PublicAPI]
    public interface ILogicallyDeletable
    {
        /// <summary>
        /// Property used to mark entity as deleted
        /// </summary>
        public bool IsDeleted { get; }
    }
}
