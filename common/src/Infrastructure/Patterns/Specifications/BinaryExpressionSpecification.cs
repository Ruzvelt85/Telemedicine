using System;
using System.Linq;
using System.Linq.Expressions;

namespace Telemedicine.Common.Infrastructure.Patterns.Specifications
{
    /// <summary>
    /// Defines specification of binary expressions
    /// </summary>
    public abstract record BinaryExpressionSpecification<T> : CompositeSpecification<T>
    {
        /// <summary>
        /// Binary expression that is used to combine two expressions
        /// </summary>
        protected Func<Expression, Expression, BinaryExpression> BinaryExpression { get; }

        /// <inheritdoc />
        protected BinaryExpressionSpecification(Func<Expression, Expression, BinaryExpression> binaryExpression, params Specification<T>[] specifications)
            : base(specifications)
        {
            BinaryExpression = binaryExpression;
        }

        /// <inheritdoc />
        protected override Expression<Func<T, bool>> CombineExpressions(Expression<Func<T, bool>> leftExpression, Expression<Func<T, bool>> rightExpression)
        {
            var leftParam = leftExpression.Parameters.First();
            var rightParam = rightExpression.Parameters.First();
            BinaryExpression binaryExpression;
            if (ReferenceEquals(leftParam, rightParam))
            {
                binaryExpression = BinaryExpression(leftExpression.Body, rightExpression.Body);
            }
            else
            {
                binaryExpression = BinaryExpression(leftExpression.Body, Expression.Invoke(rightExpression, leftParam));
            }

            return Expression.Lambda<Func<T, bool>>(binaryExpression, leftParam);
        }
    }
}
