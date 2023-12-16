using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Xunit;
using Telemedicine.Services.WebClientBffService.API.Providers.ProviderQueryService.Dto;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Validators.Providers
{
    public class ProviderListFilterRequestValidatorTests
    {
        private readonly ProviderListFilterRequestDto _defaultModel;
        private readonly ProviderListFilterRequestDtoValidator _dtoValidator;

        public ProviderListFilterRequestValidatorTests()
        {
            _defaultModel = new ProviderListFilterRequestDto();
            _dtoValidator = new ProviderListFilterRequestDtoValidator();
        }

        [Fact]
        public async Task Name_Default_ShouldNotHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldNotHaveValidationErrorFor(_ => _.Name);
        }

        [Fact]
        public async Task Name_Null_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Name = null };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Name);
        }

        [Fact]
        public async Task Name_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Name = "Abc-De Efg's" };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Name);
        }

        [Fact]
        public async Task Name_WithCorrectMaxLength_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Name = new string('a', 101) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Name);
        }

        [Fact]
        public async Task Name_WithLengthMoreThanMaxLength_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Name = new string('a', 102) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Name);
        }
    }
}
