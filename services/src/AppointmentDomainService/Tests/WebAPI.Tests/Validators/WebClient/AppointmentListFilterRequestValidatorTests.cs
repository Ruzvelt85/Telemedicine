using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.Validators.WebClient
{
    public class AppointmentListFilterRequestValidatorTests
    {
        private readonly AppointmentListFilterRequestDto _defaultModel;
        private readonly AppointmentListFilterRequestDtoValidator _dtoValidator;

        public AppointmentListFilterRequestValidatorTests()
        {
            _defaultModel = new AppointmentListFilterRequestDto();
            _dtoValidator = new AppointmentListFilterRequestDtoValidator();
        }

        [Fact]
        public async Task DoctorId_Default_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.AttendeeId);
        }

        [Fact]
        public async Task DoctorId_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { AttendeeId = Guid.NewGuid() };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.AttendeeId);
        }

        [Fact]
        public async Task AppointmentStatusFilter_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { AppointmentStates = new[] { AppointmentState.Opened } };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.AppointmentStates);
        }

        [Fact]
        public async Task AppointmentStatusFilter_Incorrect_ShouldHaveValidationError()
        {
            var model = _defaultModel with { AppointmentStates = new[] { (AppointmentState)11 } };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.AppointmentStates);
        }

        [Fact]
        public async Task DateRange_Default_ShouldNotHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldNotHaveValidationErrorFor(_ => _.DateRange);
        }

        [Fact]
        public async Task DateRange_Null_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { DateRange = new Range<DateTime?>(null, null) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.DateRange);
        }

        [Fact]
        public async Task DateRange_NullFrom_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { DateRange = new Range<DateTime?>(null, new DateTime(2021, 11, 30)) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.DateRange);
        }

        [Fact]
        public async Task DateRange_NullTo_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { DateRange = new Range<DateTime?>(new DateTime(2021, 11, 01), null) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.DateRange);
        }

        [Fact]
        public async Task DateRange_NotBlank_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { DateRange = new Range<DateTime?>(new DateTime(2021, 11, 01), new DateTime(2021, 11, 30)) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.DateRange);
        }

        [Fact]
        public async Task State_Single_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { AppointmentStates = new[] { AppointmentState.Opened } };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.AppointmentStates);
        }

        [Fact]
        public async Task State_Empty_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { AppointmentStates = Array.Empty<AppointmentState>() };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.AppointmentStates);
        }

        [Fact]
        public async Task State_Null_ShouldNotHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldNotHaveValidationErrorFor(_ => _.AppointmentStates);
        }

        [Fact]
        public async Task State_Multi_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel with { AppointmentStates = new[] { AppointmentState.Opened, AppointmentState.Missed } });

            result.ShouldNotHaveValidationErrorFor(_ => _.AppointmentStates);
        }

        [Fact]
        public async Task State_Double_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel with { AppointmentStates = new[] { AppointmentState.Opened, AppointmentState.Opened } });

            result.ShouldHaveValidationErrorFor(_ => _.AppointmentStates);
        }

        [Fact]
        public async Task State_Default_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel with { AppointmentStates = new[] { AppointmentState.Default } });

            result.ShouldHaveValidationErrorFor(_ => _.AppointmentStates);
        }

        [Fact]
        public async Task State_NotIntEnum_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel with { AppointmentStates = new[] { (AppointmentState)int.MaxValue } });

            result.ShouldHaveValidationErrorFor(_ => _.AppointmentStates);
        }

        [Fact]
        public async Task DateRange_NotBlankIncorrect_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { DateRange = new Range<DateTime?>(new DateTime(2021, 11, 20), new DateTime(2021, 11, 18)) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.DateRange.To);
        }
    }
}
