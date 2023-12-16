using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.Validators.WebClient
{
    public class AppointmentListRequestDtoValidatorTests
    {
        private readonly AppointmentListRequestDto _defaultModel;
        private readonly AppointmentListRequestDtoValidator _validator;

        public AppointmentListRequestDtoValidatorTests()
        {
            _defaultModel = new AppointmentListRequestDto()
            {
                Paging = new PagingRequestDto(),
                Filter = new AppointmentListFilterRequestDto(),
            };
            _validator = new AppointmentListRequestDtoValidator();
        }

        [Fact]
        public async Task Model_Default_ShouldNotHaveValidationErrorFor()
        {
            var result = await _validator.TestValidateAsync(_defaultModel);

            result.ShouldNotHaveValidationErrorFor(_ => _.Paging);
            result.ShouldNotHaveValidationErrorFor(_ => _.Filter);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task Model_Correct_ShouldNotHaveValidationErrorFor()
        {
            var model = _defaultModel with
            {
                Filter = new AppointmentListFilterRequestDto
                {
                    DateRange = new Range<DateTime?>(new DateTime(2021, 11, 01), new DateTime(2021, 11, 30)),
                    AttendeeId = Guid.NewGuid(),
                    AppointmentStates = new[] { AppointmentState.Opened }
                },
                Paging = new PagingRequestDto(100, 10)
            };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Paging);
            result.ShouldNotHaveValidationErrorFor(_ => _.Filter);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Paging_NestedProperty_Take_ShouldHaveValidationErrorFor()
        {
            var model = _defaultModel with
            {
                Paging = new PagingRequestDto(101, 10)
            };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Paging.Take);
            result.ShouldHaveAnyValidationError();
        }
    }
}
