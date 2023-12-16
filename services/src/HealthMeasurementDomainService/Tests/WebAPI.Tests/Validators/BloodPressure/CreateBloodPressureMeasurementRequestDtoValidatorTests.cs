using Xunit;
using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.Tests.Validators.BloodPressure
{
    public class CreateBloodPressureMeasurementRequestDtoValidatorTests
    {
        private readonly CreateMeasurementRequestDto<BloodPressureMeasurementDto> _defaultModel;
        private readonly CreateBloodPressureMeasurementRequestDtoValidator _dtoValidator;

        public CreateBloodPressureMeasurementRequestDtoValidatorTests()
        {
            _defaultModel = new CreateMeasurementRequestDto<BloodPressureMeasurementDto>();
            _dtoValidator = new CreateBloodPressureMeasurementRequestDtoValidator();
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
