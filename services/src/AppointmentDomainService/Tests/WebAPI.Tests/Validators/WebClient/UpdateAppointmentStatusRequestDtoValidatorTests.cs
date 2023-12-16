using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.Validators.WebClient
{
    public class UpdateAppointmentStatusRequestDtoValidatorTests
    {
        private readonly UpdateAppointmentStatusRequestDto _defaultModel;
        private readonly UpdateAppointmentStatusRequestDtoValidator _requestValidator;

        public UpdateAppointmentStatusRequestDtoValidatorTests()
        {
            _defaultModel = new UpdateAppointmentStatusRequestDto();
            _requestValidator = new UpdateAppointmentStatusRequestDtoValidator();
        }

        [Fact]
        public async Task Model_Default_ShouldHaveAnyValidationError()
        {
            var result = await _requestValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.Id);
            result.ShouldHaveValidationErrorFor(_ => _.Status);
            result.ShouldNotHaveValidationErrorFor(_ => _.Reason);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task Model_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Id = Guid.NewGuid(), Reason = "Qwerty", Status = AppointmentStatus.Cancelled };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Id);
            result.ShouldNotHaveValidationErrorFor(_ => _.Status);
            result.ShouldNotHaveValidationErrorFor(_ => _.Reason);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Id_Default_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Id = Guid.Empty };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Id);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task Reason_DefaultAndNotCancelledStatus_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Status = AppointmentStatus.Default, Reason = string.Empty };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Reason);
        }

        [Fact]
        public async Task Reason_DefaultAndCancelledStatus_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Status = AppointmentStatus.Cancelled, Reason = string.Empty };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Reason);
        }

        [Fact]
        public async Task Reason_LongerThanPossible_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Reason = new string('a', 101), Status = AppointmentStatus.Cancelled };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Reason);
        }

        [Fact]
        public async Task Reason_WithMaxPossibleLength_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Reason = new string('a', 100), Status = AppointmentStatus.Cancelled };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Reason);
        }

        [Fact]
        public async Task Status_Default_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Status = AppointmentStatus.Default };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Status);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task Status_NotInEnum_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Status = (AppointmentStatus)100 };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Status);
        }
    }
}
