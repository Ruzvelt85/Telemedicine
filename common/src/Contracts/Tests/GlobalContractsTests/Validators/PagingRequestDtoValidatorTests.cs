using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Xunit;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Common.Contracts.Tests.GlobalContracts.Validators
{
    public class PagingRequestDtoValidatorTests
    {
        private readonly PagingRequestDto _defaultModel;
        private readonly PagingRequestDtoValidator _validator;

        public PagingRequestDtoValidatorTests()
        {
            _defaultModel = new PagingRequestDto();
            _validator = new PagingRequestDtoValidator();
        }

        [Fact]
        public async Task Skip_Default_ShouldNotHaveValidationError()
        {
            var result = await _validator.TestValidateAsync(_defaultModel);

            result.ShouldNotHaveValidationErrorFor(_ => _.Skip);
        }

        [Fact]
        public async Task Skip_Zero_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Skip = 0 };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Skip);
        }

        [Fact]
        public async Task Skip_Negative_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Skip = -1 };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Skip);
        }

        [Fact]
        public async Task Take_Default_ShouldNotHaveValidationError()
        {
            var result = await _validator.TestValidateAsync(_defaultModel);

            result.ShouldNotHaveValidationErrorFor(_ => _.Take);
        }

        [Fact]
        public async Task Take_EqualMinValue_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Take = 0 };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Take);
        }

        [Fact]
        public async Task Take_EqualMaxValue_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Take = 100 };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Take);
        }

        [Fact]
        public async Task Take_Negative_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Take = -1 };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Take);
        }

        [Fact]
        public async Task Take_MoreThanMaxValue_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Take = 101 };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Take);
        }
    }
}
