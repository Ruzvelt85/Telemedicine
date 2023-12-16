using System.Linq.Expressions;

namespace Telemedicine.Common.Infrastructure.Patterns.Specifications
{
    /// <summary>
    /// Defines specification that is used to combine specifications by OR operator
    /// </summary>
    public record OrSpecification<T> : BinaryExpressionSpecification<T>
    {
        public OrSpecification(params Specification<T>[] specifications)
            : base(Expression.OrElse, specifications)
        {
        }
    }
}
