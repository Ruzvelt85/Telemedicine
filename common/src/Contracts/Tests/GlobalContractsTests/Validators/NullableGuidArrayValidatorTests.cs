using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Xunit;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Common.Contracts.Tests.GlobalContracts.Validators
{
    public class NullableGuidArrayValidatorTests
    {
        private const int MaxArrayLength = 100;

        private readonly NullableGuidArrayValidator _validator;

        public NullableGuidArrayValidatorTests()
        {
            _validator = new NullableGuidArrayValidator(MaxArrayLength);
        }

        [Fact]
        public async Task GuidArray_Empty_ShouldNotHaveValidationError()
        {
            var array = Array.Empty<Guid>();
            var result = await _validator.TestValidateAsync(array);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(MaxArrayLength / 2)]
        [InlineData(MaxArrayLength)]
        public async Task GuidArray_ValidArraySizeWithNonEmptyItems_ShouldNotHaveErrors(int arraySize)
        {
            var array = Enumerable.Range(0, arraySize).Select(_ => Guid.NewGuid()).ToArray();
            var result = await _validator.TestValidateAsync(array);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(MaxArrayLength / 2)]
        [InlineData(MaxArrayLength)]
        public async Task GuidArray_ValidArraySizeWithEmptyItems_ShouldHaveErrors(int arraySize)
        {
            var array = Enumerable.Range(0, arraySize).Select(_ => Guid.Empty).ToArray();
            var result = await _validator.TestValidateAsync(array);

            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task GuidArray_ValidArraySizeWithOneEmptyValue_ShouldHaveValidationError()
        {
            var array = new[] { Guid.NewGuid(), Guid.Empty, Guid.NewGuid() };
            var result = await _validator.TestValidateAsync(array);

            result.ShouldHaveAnyValidationError();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(MaxArrayLength / 2)]
        [InlineData(MaxArrayLength)]
        public async Task GuidArray_LengthIsLessOrEqualToMaxSize_ShouldNotHaveValidationError(int maxSize)
        {
            var array = Enumerable.Range(0, maxSize).Select(_ => Guid.NewGuid()).ToArray();
            var result = await _validator.TestValidateAsync(array);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(MaxArrayLength + 1)]
        [InlineData(MaxArrayLength + (MaxArrayLength / 2))]
        [InlineData(MaxArrayLength + MaxArrayLength)]
        public async Task GuidArray_LengthIsGreaterThanMaxSize_ShouldHaveValidationError(int maxSize)
        {
            var array = Enumerable.Range(0, maxSize).Select(_ => Guid.NewGuid()).ToArray();
            var result = await _validator.TestValidateAsync(array);

            result.ShouldHaveAnyValidationError();
        }
    }
}
