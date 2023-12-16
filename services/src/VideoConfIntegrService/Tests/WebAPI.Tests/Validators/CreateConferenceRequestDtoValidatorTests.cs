using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Xunit;
using Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceCommandService.Dto;

namespace Telemedicine.Services.VideoConfIntegrService.Tests.WebAPI.Tests.Validators
{
    public class CreateConferenceRequestDtoValidatorTests
    {
        private readonly CreateConferenceRequestDto _defaultModel;
        private readonly CreateConferenceRequestDtoValidator _requestValidator;

        public CreateConferenceRequestDtoValidatorTests()
        {
            _defaultModel = new CreateConferenceRequestDto();
            _requestValidator = new CreateConferenceRequestDtoValidator();
        }

        [Fact]
        public async Task Model_Default_ShouldHaveValidationErrors()
        {
            var result = await _requestValidator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.AppointmentId);
            result.ShouldHaveValidationErrorFor(_ => _.AppointmentTitle);
            result.ShouldHaveValidationErrorFor(_ => _.AppointmentStartDate);
        }

        [Fact]
        public async Task Model_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with
            {
                AppointmentId = Guid.NewGuid(),
                AppointmentTitle = "Title",
                AppointmentStartDate = DateTime.UtcNow.AddDays(1),
                AppointmentDuration = TimeSpan.FromMinutes(30)
            };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.AppointmentId);
            result.ShouldNotHaveValidationErrorFor(_ => _.AppointmentTitle);
            result.ShouldNotHaveValidationErrorFor(_ => _.AppointmentStartDate);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task AppointmentId_Default_ShouldHaveValidationError()
        {
            var model = _defaultModel with { AppointmentId = Guid.Empty };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.AppointmentId);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task AppointmentTitle_Null_ShouldHaveValidationError()
        {
            var model = _defaultModel with { AppointmentTitle = null };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.AppointmentTitle);
        }

        [Fact]
        public async Task AppointmentTitle_TooBig_ShouldHaveValidationError()
        {
            var model = _defaultModel with { AppointmentTitle = new string('A', 101) };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.AppointmentTitle);
        }

        [Fact]
        public async Task AppointmentDuration_Zero_ShouldHaveValidationError()
        {
            var model = _defaultModel with { AppointmentDuration = TimeSpan.Zero };
            var result = await _requestValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.AppointmentDuration);
        }
    }
}
