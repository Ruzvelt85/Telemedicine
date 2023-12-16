using System;
using System.Linq;
using System.Linq.Expressions;

namespace Telemedicine.Common.Infrastructure.Patterns.Specifications
{
    /// <summary>
    /// Defines specification that is used in the negation of the primary specification to its logically opposite specification
    /// </summary>
    public record NotSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _specification;

        public NotSpecification(Specification<T> specification)
        {
            _specification = specification;
        }

        /// <inheritdoc />
        protected override Expression<Func<T, bool>> Predicate
        {
            get
            {
                Expression<Func<T, bool>> expression = _specification;
                var param = expression.Parameters.First();
                var notExpression = Expression.Not(expression.Body);

                return Expression.Lambda<Func<T, bool>>(notExpression, param);
            }
        }
    }
}
