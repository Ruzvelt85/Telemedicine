using System;
using System.Linq;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Specifications.ExpressionTree;

namespace Telemedicine.Common.Infrastructure.Patterns.Specifications
{
    public record AsSpecification<TOld, TNew> : Specification<TNew>
    {
        private readonly Specification<TOld> _specification;

        public AsSpecification(Specification<TOld> specification)
        {
            _specification = specification;
        }

        /// <inheritdoc />
        protected override Expression<Func<TNew, bool>> Predicate
        {
            get
            {
                Expression<Func<TOld, bool>> oldExpression = _specification;
                var oldParameter = oldExpression.Parameters.First();
                var newParameter = Expression.Parameter(typeof(TNew));
                var visitor = new ChangeExpressionParameterVisitor(oldParameter, newParameter);
                var newExpression = visitor.Visit(oldExpression.Body);

                return Expression.Lambda<Func<TNew, bool>>(newExpression, newParameter);
            }
        }
    }
}
