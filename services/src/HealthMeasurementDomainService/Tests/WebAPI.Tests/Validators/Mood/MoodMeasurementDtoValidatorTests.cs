using Xunit;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.Tests.Validators.Mood
{
    public class MoodMeasurementDtoValidatorTests
    {
        private readonly MoodMeasurementDto _defaultModel;
        private readonly MoodMeasurementDtoValidator _dtoValidator;

        public MoodMeasurementDtoValidatorTests()
        {
            _defaultModel = new MoodMeasurementDto();
            _dtoValidator = new MoodMeasurementDtoValidator();
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
    }
}
