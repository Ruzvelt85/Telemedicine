using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Xunit;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.Tests.Validators.Mood
{
    public class CreateMoodMeasurementRequestDtoValidatorTests
    {
        private readonly CreateMeasurementRequestDto<MoodMeasurementDto> _defaultModel;
        private readonly CreateMoodMeasurementRequestDtoValidator _dtoValidator;

        public CreateMoodMeasurementRequestDtoValidatorTests()
        {
            _defaultModel = new CreateMeasurementRequestDto<MoodMeasurementDto>();
            _dtoValidator = new CreateMoodMeasurementRequestDtoValidator();
        }

        [Fact]
        public async Task AllProperties_Default_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.PatientId);
            result.ShouldHaveValidationErrorFor(_ => _.ClientDate);
        }

        [Fact]
        public async Task PatientId_Empty_ShouldHaveValidationError()
        {
            var model = _defaultModel with { PatientId = Guid.Empty };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.PatientId);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task PatientId_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { PatientId = Guid.NewGuid() };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.PatientId);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task ClientDate_Empty_ShouldHaveValidationError()
        {
            var model = _defaultModel with { ClientDate = new DateTime() };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.ClientDate);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task ClientDate_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { ClientDate = DateTime.Now };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.ClientDate);
        }
    }
}
