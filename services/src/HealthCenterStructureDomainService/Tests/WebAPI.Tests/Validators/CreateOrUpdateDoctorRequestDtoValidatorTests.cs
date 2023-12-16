using FluentValidation.TestHelper;
using System.Threading.Tasks;
using Xunit;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsCommandService.Dto;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.Validators
{
    public class CreateOrUpdateDoctorRequestDtoValidatorTests
    {
        private readonly CreateOrUpdateDoctorRequestDto _defaultModel;
        private readonly CreateOrUpdateDoctorRequestDtoValidator _requestValidator;

        public CreateOrUpdateDoctorRequestDtoValidatorTests()
        {
            _defaultModel = new CreateOrUpdateDoctorRequestDto();
            _requestValidator = new CreateOrUpdateDoctorRequestDtoValidator();
        }

        [Fact]
        public async Task InnerId_Empty_ShouldHaveValidationError()
        {
            var result = await _requestValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.InnerId);
        }

        [Fact]
        public async Task InnerId_Null_ShouldHaveValidationError()
        {
            var model = _defaultModel with { InnerId = null! };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.InnerId);
        }


        [Fact]
        public async Task InnerId_TooLong_ShouldHaveValidationError()
        {
            var model = _defaultModel with { InnerId = new string('1', 56) };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.InnerId);
        }

        [Fact]
        public async Task InnerId_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { InnerId = new string('1', 55) };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.InnerId);
        }

        [Fact]
        public async Task FirstName_Empty_ShouldHaveValidationError()
        {
            var result = await _requestValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.FirstName);
        }

        [Fact]
        public async Task FirstName_Null_ShouldHaveValidationError()
        {
            var model = _defaultModel with { FirstName = null! };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.FirstName);
        }

        [Fact]
        public async Task FirstName_TooLong_ShouldHaveValidationError()
        {
            var model = _defaultModel with { FirstName = new string('1', 51) };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.FirstName);
        }

        [Fact]
        public async Task FirstName_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { FirstName = new string('1', 50) };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.FirstName);
        }

        [Fact]
        public async Task LastName_Empty_ShouldHaveValidationError()
        {
            var result = await _requestValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.LastName);
        }

        [Fact]
        public async Task LastName_Null_ShouldHaveValidationError()
        {
            var model = _defaultModel with { FirstName = null! };
            var result = await _requestValidator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(_ => _.LastName);
        }

        [Fact]
        public async Task LastName_TooLong_ShouldHaveValidationError()
        {
            var model = _defaultModel with { LastName = new string('1', 51) };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.LastName);
        }

        [Fact]
        public async Task LastName_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { LastName = new string('1', 50) };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.LastName);
        }

        [Fact]
        public async Task HealthCenters_Empty_ShouldHaveValidationError()
        {
            var result = await _requestValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.HealthCenterInnerIds);
        }

        [Fact]
        public async Task HealthCenters_Null_ShouldHaveValidationError()
        {
            var model = _defaultModel with { HealthCenterInnerIds = null! };
            var result = await _requestValidator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(_ => _.HealthCenterInnerIds);
        }

        [Fact]
        public async Task HealthCenters_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { HealthCenterInnerIds = new[] { "test health center" } };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.HealthCenterInnerIds);
        }

        [Fact]
        public async Task IsActive_Null_ShouldHaveValidationError()
        {
            var model = _defaultModel with { IsActive = null! };
            var result = await _requestValidator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(_ => _.HealthCenterInnerIds);
        }

        [Fact]
        public async Task IsActive_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { IsActive = true };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.IsActive);
        }

        [Fact]
        public async Task Model_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with
            {
                InnerId = "Test",
                FirstName = "John",
                LastName = "Galt",
                HealthCenterInnerIds = new[] { "health_center_inner_id" },
                IsActive = false
            };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.InnerId);
            result.ShouldNotHaveValidationErrorFor(_ => _.FirstName);
            result.ShouldNotHaveValidationErrorFor(_ => _.LastName);
            result.ShouldNotHaveValidationErrorFor(_ => _.HealthCenterInnerIds);
            result.ShouldNotHaveValidationErrorFor(_ => _.IsActive);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
