using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Xunit;
using Telemedicine.Services.MobileClientBffService.API;
using Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto;

namespace Telemedicine.Services.MobileClientBffService.Tests.WebAPI.Tests.Validators
{
    public class CreateMoodMeasurementRequestDtoValidatorTests
    {
        private readonly CreateMoodMeasurementRequestDto _defaultModel;
        private readonly CreateMoodMeasurementRequestDtoValidator _dtoValidator;

        public CreateMoodMeasurementRequestDtoValidatorTests()
        {
            _defaultModel = new CreateMoodMeasurementRequestDto();
            _dtoValidator = new CreateMoodMeasurementRequestDtoValidator();
        }

        [Fact]
        public async Task AllProperties_Default_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.Measure);
            result.ShouldHaveValidationErrorFor(_ => _.ClientDate);
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

        [Fact]
        public async Task Measure_Empty_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Measure = MoodMeasureType.Default };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Measure);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task Measure_OutOfEnum_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Measure = (MoodMeasureType)int.MaxValue };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Measure);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task Measure_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Measure = MoodMeasureType.Neutral };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Measure);
        }

        [Fact]
        public async Task Model_Correct_ShouldNotHaveAnyValidationErrors()
        {
            var model = new CreateMoodMeasurementRequestDto
            {
                ClientDate = DateTime.Now,
                Measure = MoodMeasureType.Satisfied
            };

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
