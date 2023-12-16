using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Telemedicine.Common.Infrastructure.Patterns.Specifications
{
    /// <summary>
    /// Defines composite specification that allows to combine several specifications
    /// </summary>
    public abstract record CompositeSpecification<T> : Specification<T>
    {
        /// <summary>
        /// List of all specifications added to composite specification
        /// </summary>
        protected List<Specification<T>> Specifications { get; }

        protected CompositeSpecification(params Specification<T>[] specifications)
        {
            Specifications = new List<Specification<T>>(specifications);
        }

        /// <inheritdoc />
        protected override Expression<Func<T, bool>> Predicate
        {
            get
            {
                if (Specifications.Count == 0)
                {
                    throw new ArgumentException("Specification list in composite specification is empty.");
                }

                Expression<Func<T, bool>> result = Specifications[0];
                for (var i = 1; i < Specifications.Count; i++)
                {
                    result = CombineExpressions(result, Specifications[i]);
                }
                return result;
            }
        }

        /// <summary>
        /// Method that allows to combine several expressions into one expression 
        /// </summary>
        /// <param name="leftExpression"></param>
        /// <param name="rightExpression"></param>
        /// <returns></returns>
        protected abstract Expression<Func<T, bool>> CombineExpressions(Expression<Func<T, bool>> leftExpression, Expression<Func<T, bool>> rightExpression);
    }
}
