using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto;
using Xunit;

namespace Telemedicine.Services.MobileClientBffService.Tests.WebAPI.Tests.Validators
{
    public class CreateBloodPressureMeasurementRequestDtoValidatorTests
    {
        private readonly CreateBloodPressureMeasurementRequestDto _defaultModel;
        private readonly CreateBloodPressureMeasurementRequestDtoValidator _dtoValidator;

        public CreateBloodPressureMeasurementRequestDtoValidatorTests()
        {
            _defaultModel = new CreateBloodPressureMeasurementRequestDto();
            _dtoValidator = new CreateBloodPressureMeasurementRequestDtoValidator();
        }

        [Fact]
        public async Task Systolic_Default_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.Systolic);
        }

        [Fact]
        public async Task Systolic_Zero_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Systolic = 0 };

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Systolic);
        }

        [Fact]
        public async Task Systolic_Negative_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Systolic = -123 };

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Systolic);
        }

        [Fact]
        public async Task Systolic_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Systolic = 120 };

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Systolic);
        }

        [Fact]
        public async Task Diastolic_Default_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.Diastolic);
        }

        [Fact]
        public async Task Diastolic_Zero_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Diastolic = 0 };

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Diastolic);
        }

        [Fact]
        public async Task Diastolic_Negative_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Diastolic = -123 };

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Diastolic);
        }

        [Fact]
        public async Task Diastolic_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Diastolic = 70 };

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Diastolic);
        }

        [Fact]
        public async Task PulseRate_Default_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.PulseRate);
        }

        [Fact]
        public async Task PulseRate_Zero_ShouldHaveValidationError()
        {
            var model = _defaultModel with { PulseRate = 0 };

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Systolic);
        }

        [Fact]
        public async Task PulseRate_Negative_ShouldHaveValidationError()
        {
            var model = _defaultModel with { PulseRate = -123 };

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.PulseRate);
        }

        [Fact]
        public async Task PulseRate_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { PulseRate = 60 };

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.PulseRate);
        }

        [Fact]
        public async Task ClientDate_Default_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.ClientDate);
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
