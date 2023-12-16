using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentCommandService.Dto;
using Xunit;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Validators.Appointments
{
    public class CancelAppointmentRequestDtoValidatorTests
    {
        private readonly CancelAppointmentRequestDto _defaultModel;
        private readonly CancelAppointmentRequestDtoValidator _requestValidator;

        public CancelAppointmentRequestDtoValidatorTests()
        {
            _defaultModel = new CancelAppointmentRequestDto();
            _requestValidator = new CancelAppointmentRequestDtoValidator();
        }

        [Fact]
        public async Task Model_Default_ShouldHaveValidationErrors()
        {
            var result = await _requestValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.Id);
            result.ShouldHaveValidationErrorFor(_ => _.Reason);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task Model_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Id = Guid.NewGuid(), Reason = "Qwerty" };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Id);
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
        public async Task Reason_Null_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Reason = null };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Reason);
        }

        [Fact]
        public async Task Reason_Empty_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Reason = string.Empty };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Reason);
        }

        [Fact]
        public async Task Reason_LongerThanPossible_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Reason = new string('a', 101) };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Reason);
        }

        [Fact]
        public async Task Reason_WithMaxPossibleLength_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Reason = new string('a', 100) };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Reason);
        }
    }
}
