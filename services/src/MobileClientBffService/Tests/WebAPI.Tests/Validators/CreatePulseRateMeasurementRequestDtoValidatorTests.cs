using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Xunit;
using Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto;

namespace Telemedicine.Services.MobileClientBffService.Tests.WebAPI.Tests.Validators
{
    public class CreatePulseRateMeasurementRequestDtoValidatorTests
    {
        private readonly CreatePulseRateMeasurementRequestDto _defaultModel;
        private readonly CreatePulseRateMeasurementRequestDtoValidator _dtoValidator;

        public CreatePulseRateMeasurementRequestDtoValidatorTests()
        {
            _defaultModel = new CreatePulseRateMeasurementRequestDto();
            _dtoValidator = new CreatePulseRateMeasurementRequestDtoValidator();
        }

        [Fact]
        public async Task Systolic_Default_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.PulseRate);
            result.ShouldHaveValidationErrorFor(_ => _.ClientDate);
        }

        [Fact]
        public async Task PulseRate_Default_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.PulseRate);
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
