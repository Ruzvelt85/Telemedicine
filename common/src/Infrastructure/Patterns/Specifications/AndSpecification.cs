using System.Linq.Expressions;

namespace Telemedicine.Common.Infrastructure.Patterns.Specifications
{
    /// <summary>
    /// Defines specification that is used to combine specifications by AND operator
    /// </summary>
    public record AndSpecification<T> : BinaryExpressionSpecification<T>
    {
        public AndSpecification(params Specification<T>[] specifications)
            : base(Expression.AndAlso, specifications)
        {
        }
    }
}
