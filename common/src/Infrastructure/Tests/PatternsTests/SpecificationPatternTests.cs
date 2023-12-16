using System;
using System.Collections.Generic;
using System.Linq;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Common.Infrastructure.Tests.PatternsTests.Specification;
using Xunit;

namespace Telemedicine.Common.Infrastructure.Tests.PatternsTests
{
    public class SpecificationPatternTests
    {
        private readonly IQueryable<int> _numbers;

        public static IEnumerable<object[]> GetTestData()
        {
            yield return new object[]
            {
                "LessThanSpecification_Test",
                new TestLessThanSpecification(5),
                new List<int> { 1, 2, 3, 4 }
            };
            yield return new object[]
            {
                "GreaterThanSpecification_Test",
                new TestGreaterThanSpecification(5),
                new List<int> { 6, 7, 8, 9, 10 }
            };
            yield return new object[]
            {
                "EqualSpecification_Test",
                new TestEqualSpecification(5),
                new List<int> { 5 }
            };
            yield return new object[]
            {
                "NotSpecification_Test",
                new NotSpecification<int>(new TestEqualSpecification(5)),
                new List<int> { 1, 2, 3, 4, 6, 7, 8, 9, 10 }
            };
            yield return new object[]
            {
                "NotSpecification_WithOperator_Test",
                !new TestEqualSpecification(5),
                new List<int> { 1, 2, 3, 4, 6, 7, 8, 9, 10 }
            };
            yield return new object[]
            {
                "AndSpecification_WithOneSpecification_Test",
                new AndSpecification<int>(new TestGreaterThanSpecification(4)),
                new List<int> { 5, 6, 7, 8, 9, 10 }
            };
            yield return new object[]
            {
                "AndSpecification_WithTwoSpecifications_Test",
                new AndSpecification<int>(new TestGreaterThanSpecification(3), new TestLessThanSpecification(7)),
                new List<int> { 4, 5, 6 }
            };
            yield return new object[]
            {
                "AndSpecification_WithThreeSpecifications_Test",
                new AndSpecification<int>(new TestGreaterThanSpecification(3), new TestEqualSpecification(5), new TestLessThanSpecification(7)),
                new List<int> { 5 }
            };
            yield return new object[]
            {
                "AndSpecification_ThreeSpecificationsWithOperator_Test",
                new AndSpecification<int>(new TestGreaterThanSpecification(3) & new TestEqualSpecification(5) & new TestLessThanSpecification(7)),
                new List<int> { 5 }
            };

            yield return new object[]
            {
                "OrSpecification_WithOneSpecification_Test",
                new OrSpecification<int>(new TestGreaterThanSpecification(3)),
                new List<int> { 4, 5, 6, 7, 8, 9, 10 }
            };
            yield return new object[]
            {
                "OrSpecification_WithTwoSpecifications_Test",
                new OrSpecification<int>(new TestGreaterThanSpecification(7), new TestLessThanSpecification(3)),
                new List<int> { 1, 2, 8, 9, 10 }
            };
            yield return new object[]
            {
                "OrSpecification_WithThreeSpecifications_Test",
                new OrSpecification<int>(new TestLessThanSpecification(3), new TestGreaterThanSpecification(7), new TestEqualSpecification(5)),
                new List<int> { 1, 2, 5, 8, 9, 10 }
            };
            yield return new object[]
            {
                "OrSpecification_ThreeSpecificationsWithOperator_Test",
                new OrSpecification<int>(new TestLessThanSpecification(3) | new TestGreaterThanSpecification(7) | new TestEqualSpecification(5)),
                new List<int> { 1, 2, 5, 8, 9, 10 }
            };
            yield return new object[]
            {
                "ComplexSpecification_Test",
                (new TestGreaterThanSpecification(4) & !new TestGreaterThanSpecification(7) & !new TestEqualSpecification(6)) | new TestEqualSpecification(1),
                new List<int> { 1, 5, 7 }
            };
        }

        public SpecificationPatternTests()
        {
            _numbers = Enumerable.Range(1, 10).AsQueryable();
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
        public void Specification_Test(string name, Specification<int> specification, List<int> expectedResult)
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
        {
            var result = _numbers.Where(specification).ToList();

            Assert.Equal(expectedResult.Count, result.Count);
            expectedResult.ForEach(expected => Assert.Contains(expected, result));
            expectedResult.ForEach(expected => Assert.True(specification.IsSatisfiedBy(expected)));
            _numbers.Except(expectedResult).ToList().ForEach(expected => Assert.False(specification.IsSatisfiedBy(expected)));
        }

        [Fact]
        public void TrueSpecification_Test()
        {
            var trueSpecification = new TrueSpecification<int>();
            var expectedResult = _numbers.ToList();

            var result = _numbers.Where(trueSpecification).ToList();

            Assert.Equal(_numbers.Count(), result.Count);
            expectedResult.ForEach(expected => Assert.Contains(expected, result));
            expectedResult.ForEach(expected => Assert.True(trueSpecification.IsSatisfiedBy(expected)));
            _numbers.Except(expectedResult).ToList().ForEach(expected => Assert.False(trueSpecification.IsSatisfiedBy(expected)));
        }

        [Fact]
        public void TrueSpecification_WithOrOperand_Test()
        {
            var trueSpecification = new TrueSpecification<int>();
            var expectedResult = _numbers.ToList();

            var result = _numbers.Where(trueSpecification | new TestGreaterThanSpecification(5)).ToList();

            Assert.Equal(_numbers.Count(), result.Count);
            expectedResult.ForEach(expected => Assert.Contains(expected, result));
            expectedResult.ForEach(expected => Assert.True(trueSpecification.IsSatisfiedBy(expected)));
            _numbers.Except(expectedResult).ToList().ForEach(expected => Assert.False(trueSpecification.IsSatisfiedBy(expected)));
        }

        [Fact]
        public void AndSpecification_WithoutSpecifications_ThrowException_Test()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            void TestCode() => _numbers.Where(new AndSpecification<int>());

            Assert.Throws<ArgumentException>(TestCode);
        }

        [Fact]
        public void OrSpecification_WithoutSpecifications_ThrowException_Test()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            void TestCode() => _numbers.Where(new OrSpecification<int>());

            Assert.Throws<ArgumentException>(TestCode);
        }
    }
}
