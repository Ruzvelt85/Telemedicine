using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Xunit;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.HealthCentersCommandService.Dto;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.Validators
{
    public class CreateOrUpdateHealthCenterRequestDtoValidatorTests
    {
        private readonly CreateOrUpdateHealthCenterRequestDto _defaultModel;
        private readonly CreateOrUpdateHealthCenterRequestDtoValidator _dtoValidator;

        public CreateOrUpdateHealthCenterRequestDtoValidatorTests()
        {
            _defaultModel = new CreateOrUpdateHealthCenterRequestDto();
            _dtoValidator = new CreateOrUpdateHealthCenterRequestDtoValidator();
        }

        [Fact]
        public async Task InnerId_Empty_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.InnerId);
        }

        [Fact]
        public async Task InnerId_Null_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { InnerId = null! };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.InnerId);
        }

        [Fact]
        public async Task InnerId_TooLong_ShouldHaveValidationError()
        {
            var model = _defaultModel with { InnerId = new string('1', 56) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.InnerId);
        }

        [Fact]
        public async Task InnerId_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { InnerId = new string('1', 55) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.InnerId);
        }

        [Fact]
        public async Task Name_Empty_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.Name);
        }

        [Fact]
        public async Task Name_Null_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Name = null! };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Name);
        }

        [Fact]
        public async Task Name_TooLong_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Name = new string('1', 65) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Name);
        }

        [Fact]
        public async Task Name_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Name = new string('1', 64) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Name);
        }

        [Fact]
        public async Task UsaState_Empty_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.UsaState);
        }

        [Fact]
        public async Task UsaState_Null_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { UsaState = null! };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.UsaState);
        }

        [Fact]
        public async Task UsaState_TooLong_ShouldHaveValidationError()
        {
            var model = _defaultModel with { UsaState = new string('1', 21) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.UsaState);
        }

        [Fact]
        public async Task UsaState_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Name = new string('1', 20) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Name);
        }

        [Fact]
        public async Task IsActive_Null_ShouldHaveValidationError()
        {
            var model = _defaultModel with { IsActive = null };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.IsActive);
        }

        [Fact]
        public async Task IsActive_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { IsActive = true };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.IsActive);
        }
    }
}
