using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Xunit;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.Tests.Validators
{
    public class MeasurementListFilterRequestDtoValidatorTests
    {
        private readonly MeasurementListFilterRequestDto _defaultModel;
        private readonly MeasurementListFilterRequestDtoValidator _dtoValidator;

        public MeasurementListFilterRequestDtoValidatorTests()
        {
            _defaultModel = new MeasurementListFilterRequestDto();
            _dtoValidator = new MeasurementListFilterRequestDtoValidator();
        }

        [Fact]
        public async Task PatientId_Default_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.PatientId);
            result.ShouldHaveAnyValidationError();
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
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task DateRange_Default_ShouldNotHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldNotHaveValidationErrorFor(_ => _.DateRange);
        }

        [Fact]
        public async Task DateRange_Empty_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { DateRange = new Range<DateTime?>() };

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.DateRange);
        }
    }
}
