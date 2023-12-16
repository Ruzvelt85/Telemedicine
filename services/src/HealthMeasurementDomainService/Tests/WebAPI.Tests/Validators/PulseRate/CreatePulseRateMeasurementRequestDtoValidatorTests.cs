using Xunit;
using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.Tests.Validators.PulseRate
{
    public class CreatePulseRateMeasurementRequestDtoValidatorTests
    {
        private readonly CreateMeasurementRequestDto<PulseRateMeasurementDto> _defaultModel;
        private readonly CreatePulseRateMeasurementRequestDtoValidator _dtoValidator;

        public CreatePulseRateMeasurementRequestDtoValidatorTests()
        {
            _defaultModel = new CreateMeasurementRequestDto<PulseRateMeasurementDto>();
            _dtoValidator = new CreatePulseRateMeasurementRequestDtoValidator();
        }

        [Fact]
        public async Task PatientId_Default_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.PatientId);
        }

        [Fact]
        public async Task PatientId_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { PatientId = Guid.NewGuid() };

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.PatientId);
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
