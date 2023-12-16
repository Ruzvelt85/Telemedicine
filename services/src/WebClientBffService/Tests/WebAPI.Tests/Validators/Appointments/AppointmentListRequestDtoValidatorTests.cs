using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentList;
using Telemedicine.Services.WebClientBffService.API.Common;
using Xunit;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Validators.Appointments
{
    public class AppointmentListRequestDtoValidatorTests
    {
        private readonly AppointmentListRequestDto _defaultRequestDto;
        private readonly AppointmentListRequestDtoValidator _validator;

        public AppointmentListRequestDtoValidatorTests()
        {
            _defaultRequestDto = new AppointmentListRequestDto
            {
                Paging = new PagingRequestDto(),
                Filter = new AppointmentListFilterRequestDto()
            };
            _validator = new AppointmentListRequestDtoValidator();
        }

        [Fact]
        public async Task RequestDto_Default_ShouldNotHaveValidationErrorFor()
        {
            var result = await _validator.TestValidateAsync(_defaultRequestDto);

            result.ShouldNotHaveValidationErrorFor(_ => _.Paging);
            result.ShouldNotHaveValidationErrorFor(_ => _.Filter);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task RequestDto_Correct_ShouldNotHaveValidationErrorFor()
        {
            var model = _defaultRequestDto with
            {
                Filter = new AppointmentListFilterRequestDto
                {
                    DateRange = new Range<DateTime?>(new DateTime(2021, 11, 01), new DateTime(2021, 11, 30)),
                    AppointmentStatus = AppointmentStatus.Opened
                },
                Paging = new PagingRequestDto(100, 10),
            };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Paging);
            result.ShouldNotHaveValidationErrorFor(_ => _.Filter);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task RequestDto_Paging_NestedProperty_Take_ShouldHaveValidationErrorFor()
        {
            var model = _defaultRequestDto with
            {
                Paging = new PagingRequestDto(101, 10)
            };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Paging.Take);
            result.ShouldHaveAnyValidationError();
        }
    }
}
