using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Telemedicine.Common.Infrastructure.Patterns.Specifications.ExpressionTree
{
    public class ChangeExpressionParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ChangeExpressionParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        [return: NotNullIfNotNull("node")]
        public override Expression? Visit(Expression? node)
        {
            if (node == _oldParameter)
                return _newParameter;

            return base.Visit(node);
        }
    }
}
