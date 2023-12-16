using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Services.MobileClientBffService.API.MobileClientQueryService.Dto;
using Xunit;

namespace Telemedicine.Services.MobileClientBffService.Tests.WebAPI.Tests.Validators
{
    public class LastChangedDataRequestDtoValidatorTests
    {
        private readonly LastChangedDataRequestDto _defaultModel;
        private readonly LastChangedDataRequestDtoValidator _dtoValidator;

        public LastChangedDataRequestDtoValidatorTests()
        {
            _defaultModel = new LastChangedDataRequestDto();
            _dtoValidator = new LastChangedDataRequestDtoValidator();
        }

        [Fact]
        public async Task AllProperties_Default_ShouldNotHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldNotHaveValidationErrorFor(_ => _.AppointmentsLastUpdate);
            result.ShouldNotHaveValidationErrorFor(_ => _.MoodLastUpdate);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task AppointmentLastUpdate_Empty_ShouldHaveValidationError()
        {
            var model = _defaultModel with { AppointmentsLastUpdate = new DateTime() };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.AppointmentsLastUpdate);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task AppointmentLastUpdate_NowValue_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { AppointmentsLastUpdate = DateTime.Now };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.AppointmentsLastUpdate);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task MoodLastUpdate_Empty_ShouldHaveValidationError()
        {
            var model = _defaultModel with { MoodLastUpdate = new DateTime() };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.MoodLastUpdate);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task MoodLastUpdateLastUpdate_NowValue_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { AppointmentsLastUpdate = DateTime.Now };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.MoodLastUpdate);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
