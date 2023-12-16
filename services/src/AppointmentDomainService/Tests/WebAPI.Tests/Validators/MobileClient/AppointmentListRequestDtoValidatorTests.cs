using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.Validators.MobileClient
{
    public class AppointmentListRequestDtoValidatorTests
    {
        private readonly AppointmentListRequestDto _defaultModel;
        private readonly AppointmentListRequestDtoValidator _dtoValidator;

        public AppointmentListRequestDtoValidatorTests()
        {
            _defaultModel = new AppointmentListRequestDto();
            _dtoValidator = new AppointmentListRequestDtoValidator();
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
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
