using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.Validators.MobileClient
{
    public class ChangedAppointmentListRequestDtoValidatorTests
    {
        private readonly ChangedAppointmentListRequestDto _defaultModel;
        private readonly ChangedAppointmentListRequestDtoValidator _dtoValidator;

        public ChangedAppointmentListRequestDtoValidatorTests()
        {
            _defaultModel = new ChangedAppointmentListRequestDto();
            _dtoValidator = new ChangedAppointmentListRequestDtoValidator();
        }

        [Fact]
        public async Task AttendeeId_Default_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.AttendeeId);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task AttendeeId_Empty_ShouldHaveValidationError()
        {
            var model = _defaultModel with { AttendeeId = Guid.Empty };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.AttendeeId);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task AttendeeId_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { AttendeeId = Guid.NewGuid() };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.AttendeeId);
        }

        [Fact]
        public async Task LastUpdate_Default_ShouldHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.LastUpdate);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task LastUpdate_Empty_ShouldHaveValidationError()
        {
            var model = _defaultModel with { LastUpdate = new DateTime() };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.LastUpdate);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task LastUpdate_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { LastUpdate = DateTime.Now };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.LastUpdate);
        }

        [Fact]
        public async Task Model_Correct_ShouldNotHaveAnyValidationErrors()
        {
            var model = _defaultModel with { AttendeeId = Guid.NewGuid(), LastUpdate = DateTime.Now };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.AttendeeId);
            result.ShouldNotHaveValidationErrorFor(_ => _.LastUpdate);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
