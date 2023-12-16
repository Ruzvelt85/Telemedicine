using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.Validators
{
    public class CreateAppointmentRequestDtoValidatorTests
    {
        private readonly CreateAppointmentRequestDto _defaultModel;
        private readonly CreateAppointmentRequestDtoValidator _validator;

        public CreateAppointmentRequestDtoValidatorTests()
        {
            _defaultModel = new CreateAppointmentRequestDto();
            _validator = new CreateAppointmentRequestDtoValidator();
        }

        [Fact]
        public async Task Model_Correct_ShouldNotHaveValidationError()
        {
            var model = new CreateAppointmentRequestDto()
            {
                Title = "Title",
                Description = "Description",
                Duration = TimeSpan.FromHours(3),
                StartDate = DateTime.Now.AddDays(1),
                CreatorId = Guid.NewGuid(),
                AppointmentType = AppointmentType.Urgent,
                AttendeeIds = new[] { Guid.NewGuid(), Guid.NewGuid() }
            };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Title);
            result.ShouldNotHaveValidationErrorFor(_ => _.Description);
            result.ShouldNotHaveValidationErrorFor(_ => _.Duration);
            result.ShouldNotHaveValidationErrorFor(_ => _.StartDate);
            result.ShouldNotHaveValidationErrorFor(_ => _.CreatorId);
            result.ShouldNotHaveValidationErrorFor(_ => _.AppointmentType);
            result.ShouldNotHaveValidationErrorFor(_ => _.AttendeeIds);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Model_Default_ShouldHaveAnyValidationError()
        {
            var result = await _validator.TestValidateAsync(_defaultModel);

            result.ShouldHaveValidationErrorFor(_ => _.Title);
            result.ShouldNotHaveValidationErrorFor(_ => _.Description);
            result.ShouldHaveValidationErrorFor(_ => _.Duration);
            result.ShouldHaveValidationErrorFor(_ => _.StartDate);
            result.ShouldHaveValidationErrorFor(_ => _.CreatorId);
            result.ShouldHaveValidationErrorFor(_ => _.AppointmentType);
            result.ShouldHaveValidationErrorFor(_ => _.AttendeeIds);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task Title_Empty_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Title = string.Empty };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Title);
        }

        [Fact]
        public async Task Title_LongerThanPossible_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Title = new string('a', 101) };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Title);
        }

        [Fact]
        public async Task Title_ShorterThanPossible_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Title = new string('a', 4) };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Title);
        }

        [Fact]
        public async Task Description_Null_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Description = null };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Description);
        }

        [Fact]
        public async Task Description_Empty_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Description = string.Empty };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Description);
        }

        [Fact]
        public async Task Description_LongerThanPossible_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Description = new string('a', 101) };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Description);
        }

        [Fact]
        public async Task Description_ShorterThanPossible_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Description = new string('a', 4) };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Description);
        }

        [Fact]
        public async Task StartDate_Default_ShouldHaveValidationError()
        {
            var model = _defaultModel with { StartDate = new DateTime() };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.StartDate);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task StartDate_LessThanPossible_ShouldHaveValidationError()
        {
            var model = _defaultModel with { StartDate = DateTime.Now.AddDays(-1) };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.StartDate);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task Duration_Zero_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Duration = TimeSpan.Zero };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Duration);
        }

        [Fact]
        public async Task Duration_NegativeValue_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Duration = TimeSpan.FromHours(-1) };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Duration);
        }

        [Fact]
        public async Task Duration_EqualToMinPossibleValue_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Duration = TimeSpan.FromMinutes(15) };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Duration);
        }

        [Fact]
        public async Task Duration_LessThanPossible_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Duration = TimeSpan.FromMinutes(14) };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Duration);
        }

        [Fact]
        public async Task Duration_GreaterThanPossible_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Duration = TimeSpan.FromHours(4) };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Duration);
        }

        [Fact]
        public async Task CreatorId_Empty_ShouldHaveValidationError()
        {
            var model = _defaultModel with { CreatorId = Guid.Empty };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.CreatorId);
        }

        [Fact]
        public async Task AppointmentType_Default_ShouldHaveValidationError()
        {
            var model = _defaultModel with { AppointmentType = AppointmentType.Default };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.AppointmentType);
        }

        [Fact]
        public async Task AppointmentType_NotInEnum_ShouldHaveValidationError()
        {
            var model = _defaultModel with { AppointmentType = (AppointmentType)int.MaxValue };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.AppointmentType);
        }

        [Fact]
        public async Task AttendeeIds_Empty_ShouldHaveValidationError()
        {
            var model = _defaultModel with { AttendeeIds = Array.Empty<Guid>() };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.AttendeeIds);
        }

        [Fact]
        public async Task AttendeeIds_WithEmptyGuid_ShouldHaveValidationError()
        {
            var model = _defaultModel with { AttendeeIds = new[] { Guid.Empty, Guid.Empty } };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.AttendeeIds);
        }

        [Fact]
        public async Task AttendeeIds_LessThan2Attendees_ShouldHaveValidationError()
        {
            var model = _defaultModel with { AttendeeIds = new[] { Guid.NewGuid() } };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.AttendeeIds);
        }

        [Fact]
        public async Task AttendeeIds_NonUniqueIds_ShouldHaveValidationError()
        {
            var attendeeId = Guid.NewGuid();
            var model = _defaultModel with { AttendeeIds = new[] { attendeeId, attendeeId } };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.AttendeeIds);
        }
    }
}
