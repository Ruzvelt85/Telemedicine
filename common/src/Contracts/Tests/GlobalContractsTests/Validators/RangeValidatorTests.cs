using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Xunit;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Common.Contracts.Tests.GlobalContracts.Validators
{
    public class RangeValidatorTests
    {
        private readonly Range<DateTime?> _defaultModel;
        private readonly RangeValidator<DateTime> _dtoValidator;

        public RangeValidatorTests()
        {
            _defaultModel = new Range<DateTime?>();
            _dtoValidator = new RangeValidator<DateTime>();
        }

        [Fact]
        public async Task Range_Default_ShouldNotHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldNotHaveValidationErrorFor(_ => _.From);
            result.ShouldNotHaveValidationErrorFor(_ => _.To);
        }

        [Fact]
        public async Task Range_WithFrom_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { From = DateTime.Now };

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.From);
            result.ShouldNotHaveValidationErrorFor(_ => _.To);
        }

        [Fact]
        public async Task Range_WithTo_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { To = DateTime.Now };

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.From);
            result.ShouldNotHaveValidationErrorFor(_ => _.To);
        }

        [Fact]
        public async Task Range_WithFromAndTo_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with
            {
                From = DateTime.Now.AddDays(-1),
                To = DateTime.Now
            };

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.From);
            result.ShouldNotHaveValidationErrorFor(_ => _.To);
        }

        [Fact]
        public async Task Range_WithFromMoreThenTo_ShouldHaveValidationError()
        {
            var model = _defaultModel with
            {
                From = DateTime.Now.AddDays(1),
                To = DateTime.Now
            };

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.To);
        }
    }
}
