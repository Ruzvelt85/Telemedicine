using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Xunit;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure.Dto;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.Tests.Validators.BloodPressure
{
    public class BloodPressureMeasurementDtoValidatorTests
    {
        private readonly BloodPressureMeasurementDto _defaultModel;
        private readonly BloodPressureMeasurementDtoValidator _dtoValidator;

        public BloodPressureMeasurementDtoValidatorTests()
        {
            _defaultModel = new BloodPressureMeasurementDto();
            _dtoValidator = new BloodPressureMeasurementDtoValidator();
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
    }
}
