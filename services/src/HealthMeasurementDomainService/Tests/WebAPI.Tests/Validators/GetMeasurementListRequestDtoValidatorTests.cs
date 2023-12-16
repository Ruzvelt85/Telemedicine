using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Xunit;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.Tests.Validators
{
    public class GetMeasurementListRequestDtoValidatorTests
    {
        private readonly GetMeasurementListRequestDto _defaultModel;
        private readonly GetMeasurementListRequestDtoValidator _dtoValidator;

        public GetMeasurementListRequestDtoValidatorTests()
        {
            _defaultModel = new GetMeasurementListRequestDto();
            _dtoValidator = new GetMeasurementListRequestDtoValidator();
        }

        [Fact]
        public async Task Filter_Default_ShouldNotHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldNotHaveValidationErrorFor(_ => _.Filter);
        }

        [Fact]
        public async Task Filter_Empty_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Filter = new MeasurementListFilterRequestDto() };

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Filter);
        }

        [Fact]
        public async Task Paging_Default_ShouldNotHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldNotHaveValidationErrorFor(_ => _.Paging);
        }

        [Fact]
        public async Task Paging_Empty_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Paging = new PagingRequestDto() };

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Paging);
        }
    }
}
