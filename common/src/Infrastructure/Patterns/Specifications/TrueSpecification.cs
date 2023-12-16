using System;
using System.Linq.Expressions;

namespace Telemedicine.Common.Infrastructure.Patterns.Specifications
{
    /// <summary>
    /// Defines specification that returns true for any item
    /// </summary>
    public record TrueSpecification<T> : Specification<T>
    {
        /// <inheritdoc />
        protected override Expression<Func<T, bool>> Predicate => item => true;
    }
}
