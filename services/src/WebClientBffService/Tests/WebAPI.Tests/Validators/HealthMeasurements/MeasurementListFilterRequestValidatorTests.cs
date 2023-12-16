using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.WebClientBffService.API.Common;
using Telemedicine.Services.WebClientBffService.API.HealthMeasurements.HealthMeasurementQueryService.Dto;
using Xunit;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Validators.HealthMeasurements
{
    public class MeasurementListFilterRequestValidatorTests
    {
        private readonly MeasurementListFilterRequestDto _defaultFilterRequestDto;
        private readonly MeasurementListFilterRequestDtoValidator _dtoValidator;

        public MeasurementListFilterRequestValidatorTests()
        {
            _defaultFilterRequestDto = new MeasurementListFilterRequestDto();
            _dtoValidator = new MeasurementListFilterRequestDtoValidator();
        }

        [Fact]
        public async Task PatientIdFilter_Default_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultFilterRequestDto);

            result.ShouldHaveValidationErrorFor(_ => _.PatientId);
        }

        [Fact]
        public async Task PatientIdFilter_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultFilterRequestDto with
            {
                DateRange = new Range<DateTime?>(DateTime.Now, DateTime.Now.AddDays(1)),
                PatientId = Guid.NewGuid(),
                MeasurementType = MeasurementType.All
            };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.PatientId);
        }

        [Fact]
        public async Task DateRange_Default_ShouldNotHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultFilterRequestDto);

            result.ShouldNotHaveValidationErrorFor(_ => _.DateRange);
        }

        [Fact]
        public async Task DateRange_Null_ShouldNotHaveValidationError()
        {
            var model = _defaultFilterRequestDto with { DateRange = new Range<DateTime?>(null, null) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.DateRange);
        }

        [Fact]
        public async Task DateRange_NullFrom_ShouldNotHaveValidationError()
        {
            var model = _defaultFilterRequestDto with { DateRange = new Range<DateTime?>(null, new DateTime(2021, 11, 30)) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.DateRange);
        }

        [Fact]
        public async Task DateRange_NullTo_ShouldNotHaveValidationError()
        {
            var model = _defaultFilterRequestDto with { DateRange = new Range<DateTime?>(new DateTime(2021, 11, 01), null) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.DateRange);
        }

        [Fact]
        public async Task DateRange_NotBlank_ShouldNotHaveValidationError()
        {
            var model = _defaultFilterRequestDto with { DateRange = new Range<DateTime?>(new DateTime(2021, 11, 01), new DateTime(2021, 11, 30)) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.DateRange);
        }

        [Fact]
        public async Task MeasurementStatusFilter_Default_ShouldNotHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultFilterRequestDto);

            result.ShouldNotHaveValidationErrorFor(_ => _.MeasurementType);
        }

        [Fact]
        public async Task MeasurementTypeFilter_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultFilterRequestDto with { MeasurementType = MeasurementType.BloodPressure };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.MeasurementType);
        }

        [Fact]
        public async Task MeasurementTypeFilter_Incorrect_ShouldHaveValidationError()
        {
            var model = _defaultFilterRequestDto with { MeasurementType = (MeasurementType)(-1) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.MeasurementType);
        }
    }
}
