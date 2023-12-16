using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.WebClientBffService.API.Common;
using Telemedicine.Services.WebClientBffService.API.HealthMeasurements.HealthMeasurementQueryService.Dto;
using Xunit;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Validators.HealthMeasurements
{
    public class MeasurementListRequestDtoValidatorTests
    {
        private readonly GetMeasurementListRequestDto _defaultRequestDto;
        private readonly MeasurementListRequestDtoValidator _validator;

        public MeasurementListRequestDtoValidatorTests()
        {
            _defaultRequestDto = new GetMeasurementListRequestDto
            {
                Paging = new PagingRequestDto(),
                Filter = new MeasurementListFilterRequestDto()
            };
            _validator = new MeasurementListRequestDtoValidator();
        }

        [Fact]
        public async Task RequestDto_Default_ShouldNotHaveValidationErrorFor()
        {
            var result = await _validator.TestValidateAsync(_defaultRequestDto);

            result.ShouldNotHaveValidationErrorFor(_ => _.Paging);
            result.ShouldNotHaveValidationErrorFor(_ => _.Filter);
        }

        [Fact]
        public async Task RequestDto_Correct_ShouldNotHaveValidationErrorFor()
        {
            var model = _defaultRequestDto with
            {
                Filter = new MeasurementListFilterRequestDto
                {
                    DateRange = new Range<DateTime?>(new DateTime(2021, 11, 01), new DateTime(2021, 11, 30)),
                    PatientId = Guid.NewGuid(),
                    MeasurementType = MeasurementType.Saturation
                },
                Paging = new PagingRequestDto(100, 10)
            };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Paging);
            result.ShouldNotHaveValidationErrorFor(_ => _.Filter);
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
        }
    }
}
