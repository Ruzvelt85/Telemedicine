using JetBrains.Annotations;

namespace Telemedicine.Common.Infrastructure.EntityBase
{
    /// <summary>
    /// Interface for using with objects that require optimistic logic functionality
    /// </summary>
    [PublicAPI]
    public interface IOptimisticLock
    {
        /// <summary>
        /// Property that stores Timestamp for checking optimistic locking
        /// </summary>
        uint Timestamp { get; }
    }
}
