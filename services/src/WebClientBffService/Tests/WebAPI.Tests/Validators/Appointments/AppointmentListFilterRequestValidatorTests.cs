using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentList;
using Telemedicine.Services.WebClientBffService.API.Common;
using Xunit;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Validators.Appointments
{
    public class AppointmentListFilterRequestValidatorTests
    {
        private readonly AppointmentListFilterRequestDto _defaultFilterRequestDto;
        private readonly AppointmentListFilterRequestDtoValidator _dtoValidator;

        public AppointmentListFilterRequestValidatorTests()
        {
            _defaultFilterRequestDto = new AppointmentListFilterRequestDto();
            _dtoValidator = new AppointmentListFilterRequestDtoValidator();
        }

        [Fact]
        public async Task AppointmentStatusFilter_Default_ShouldNotHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultFilterRequestDto);

            result.ShouldNotHaveValidationErrorFor(_ => _.AppointmentStatus);
        }

        [Fact]
        public async Task AppointmentStatusFilter_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultFilterRequestDto with { AppointmentStatus = AppointmentStatus.Done };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.AppointmentStatus);
        }

        [Fact]
        public async Task AppointmentStatusFilter_Incorrect_ShouldHaveValidationError()
        {
            var model = _defaultFilterRequestDto with { AppointmentStatus = (AppointmentStatus)11 };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.AppointmentStatus);
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
        public async Task DateRange_NotBlankIncorrect_ShouldNotHaveValidationError()
        {
            var model = _defaultFilterRequestDto with { DateRange = new Range<DateTime?>(new DateTime(2021, 11, 20), new DateTime(2021, 11, 18)) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.DateRange.To);
        }
    }
}
