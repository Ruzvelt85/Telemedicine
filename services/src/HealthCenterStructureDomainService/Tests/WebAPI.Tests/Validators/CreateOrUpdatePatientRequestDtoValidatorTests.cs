using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Xunit;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsCommandService.Dto;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.Validators
{
    public class CreateOrUpdatePatientRequestDtoValidatorTests
    {
        private readonly CreateOrUpdatePatientRequestDto _defaultModel;
        private readonly CreateOrUpdatePatientRequestDtoValidator _dtoValidator;

        public CreateOrUpdatePatientRequestDtoValidatorTests()
        {
            _defaultModel = new CreateOrUpdatePatientRequestDto();
            _dtoValidator = new CreateOrUpdatePatientRequestDtoValidator();
        }

        [Fact]
        public async Task InnerId_Empty_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.InnerId);
        }

        [Fact]
        public async Task InnerId_Null_ShouldHaveValidationError()
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
        public async Task LastName_Empty_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.LastName);
        }

        [Fact]
        public async Task LastName_Null_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { LastName = null! };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.LastName);
        }

        [Fact]
        public async Task LastName_TooLong_ShouldHaveValidationError()
        {
            var model = _defaultModel with { LastName = new string('1', 51) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.LastName);
        }

        [Fact]
        public async Task LastName_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { LastName = new string('1', 50) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.LastName);
        }

        [Fact]
        public async Task FirstName_Empty_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.FirstName);
        }

        [Fact]
        public async Task FirstName_Null_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { FirstName = null! };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.FirstName);
        }

        [Fact]
        public async Task FirstName_TooLong_ShouldHaveValidationError()
        {
            var model = _defaultModel with { FirstName = new string('1', 51) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.FirstName);
        }

        [Fact]
        public async Task FirstName_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { FirstName = new string('1', 50) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.FirstName);
        }

        [Fact]
        public async Task PhoneNumber_Null_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { PhoneNumber = null };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.PhoneNumber);
        }

        [Fact]
        public async Task PhoneNumber_Empty_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { PhoneNumber = string.Empty };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.PhoneNumber);
        }

        [Fact]
        public async Task PhoneNumber_TooLong_ShouldHaveValidationError()
        {
            var model = _defaultModel with { PhoneNumber = new string('1', 33) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.PhoneNumber);
        }

        [Fact]
        public async Task PhoneNumber_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { PhoneNumber = new string('1', 32) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.PhoneNumber);
        }

        [Fact]
        public async Task BirthDate_Null_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { BirthDate = null };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.BirthDate);
        }

        [Fact]
        public async Task BirthDate_Empty_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { BirthDate = DateTime.MinValue };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.BirthDate);
        }

        [Fact]
        public async Task BirthDate_NotEmpty_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { BirthDate = new DateTime(2021, 12, 12) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.BirthDate);
        }

        [Fact]
        public async Task HealthCenterInnerId_Empty_ShouldHaveValidationError()
        {
            var model = _defaultModel with { HealthCenterInnerId = string.Empty };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.HealthCenterInnerId);
        }

        [Fact]
        public async Task HealthCenterInnerId_Null_ShouldHaveValidationError()
        {
            var model = _defaultModel with { HealthCenterInnerId = null! };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.HealthCenterInnerId);
        }

        [Fact]
        public async Task HealthCenterInnerId_TooLong_ShouldHaveValidationError()
        {
            var model = _defaultModel with { HealthCenterInnerId = new string('1', 37) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.HealthCenterInnerId);
        }

        [Fact]
        public async Task HealthCenterInnerId_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { HealthCenterInnerId = new string('1', 36) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.HealthCenterInnerId);
        }

        [Fact]
        public async Task PrimaryCareProviderInnerId_Empty_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { PrimaryCareProviderInnerId = string.Empty };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.PrimaryCareProviderInnerId);
        }

        [Fact]
        public async Task PrimaryCareProviderInnerId_Null_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { PrimaryCareProviderInnerId = null };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.PrimaryCareProviderInnerId);
        }

        [Fact]
        public async Task PrimaryCareProviderInnerId_TooLong_ShouldHaveValidationError()
        {
            var model = _defaultModel with { PrimaryCareProviderInnerId = new string('1', 37) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.PrimaryCareProviderInnerId);
        }

        [Fact]
        public async Task PrimaryCareProviderInnerId_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { PrimaryCareProviderInnerId = new string('1', 36) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.PrimaryCareProviderInnerId);
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
