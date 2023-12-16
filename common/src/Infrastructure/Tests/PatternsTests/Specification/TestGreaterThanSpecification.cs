using System;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;

namespace Telemedicine.Common.Infrastructure.Tests.PatternsTests.Specification
{
    /// <summary>
    /// Class for tests purpose; represents greater than value specification
    /// </summary>
    internal record TestGreaterThanSpecification : Specification<int>
    {
        private readonly int _value;

        public TestGreaterThanSpecification(int value)
        {
            _value = value;
        }

        /// <inheritdoc />
        protected override Expression<Func<int, bool>> Predicate => v => v > _value;
    }
}
