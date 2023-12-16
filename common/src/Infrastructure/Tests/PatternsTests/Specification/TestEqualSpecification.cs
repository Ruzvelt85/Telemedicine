using System;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;

namespace Telemedicine.Common.Infrastructure.Tests.PatternsTests.Specification
{
    /// <summary>
    /// Class for tests purpose; represents equal specification
    /// </summary>
    internal record TestEqualSpecification : Specification<int>
    {
        private readonly int _value;

        public TestEqualSpecification(int value)
        {
            _value = value;
        }

        /// <inheritdoc />
        protected override Expression<Func<int, bool>> Predicate => v => v == _value;
    }
}
