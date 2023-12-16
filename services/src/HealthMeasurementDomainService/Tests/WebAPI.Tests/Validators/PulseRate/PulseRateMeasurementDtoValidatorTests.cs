using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Xunit;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate.Dto;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.Tests.Validators.PulseRate
{
    public class PulseRateMeasurementDtoValidatorTests
    {
        private readonly PulseRateMeasurementDto _defaultModel;
        private readonly PulseRateMeasurementDtoValidator _dtoValidator;

        public PulseRateMeasurementDtoValidatorTests()
        {
            _defaultModel = new PulseRateMeasurementDto();
            _dtoValidator = new PulseRateMeasurementDtoValidator();
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
