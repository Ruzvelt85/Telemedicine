namespace Telemedicine.Common.Infrastructure.Patterns.Specifications
{
    /// <summary>
    /// Interface of specification
    /// </summary>
    public interface ISpecification<in T>
    {
        /// <summary>
        /// Returns true if item is satisfied by this specification
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool IsSatisfiedBy(T item);
    }
}
