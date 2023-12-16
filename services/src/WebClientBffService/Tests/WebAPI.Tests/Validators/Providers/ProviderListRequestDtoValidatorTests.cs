using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Xunit;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.WebClientBffService.API.Providers.ProviderQueryService.Dto;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Validators.Providers
{
    public class ProviderListRequestDtoValidatorTests
    {
        private readonly ProviderListRequestDto _defaultModel;
        private readonly ProviderListRequestDtoValidator _validator;

        public ProviderListRequestDtoValidatorTests()
        {
            _defaultModel = new ProviderListRequestDto
            {
                Paging = new PagingRequestDto(),
                Filter = new ProviderListFilterRequestDto()
            };
            _validator = new ProviderListRequestDtoValidator();
        }

        [Fact]
        public async Task Model_Default_ShouldNotHaveValidationErrorFor()
        {
            var result = await _validator.TestValidateAsync(_defaultModel);

            result.ShouldNotHaveValidationErrorFor(_ => _.FirstNameSortingType);
            result.ShouldNotHaveValidationErrorFor(_ => _.Paging);
            result.ShouldNotHaveValidationErrorFor(_ => _.Filter);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task FirstNameSortingType_CorrectValue_ShouldNotHaveValidationErrorFor()
        {
            var model = _defaultModel with { FirstNameSortingType = SortingType.Desc };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.FirstNameSortingType);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task FirstNameSortingType_IncorrectValue_ShouldHaveValidationErrorFor()
        {
            var model = _defaultModel with { FirstNameSortingType = (SortingType)11 };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.FirstNameSortingType);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task Model_Correct_ShouldNotHaveValidationErrorFor()
        {
            var model = _defaultModel with
            {
                Filter = new ProviderListFilterRequestDto
                {
                    Name = new string('a', 101),
                    HealthCenterIds = new[] { Guid.NewGuid() }
                },
                Paging = new PagingRequestDto(100, 10),
                FirstNameSortingType = SortingType.Desc
            };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Paging);
            result.ShouldNotHaveValidationErrorFor(_ => _.Filter);
            result.ShouldNotHaveValidationErrorFor(_ => _.FirstNameSortingType);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Filter_NestedProperty_Name_ShouldHaveValidationErrorFor()
        {
            var model = _defaultModel with
            {
                Filter = new ProviderListFilterRequestDto
                {
                    Name = new string('a', 102)
                }
            };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Filter!.Name);
            result.ShouldHaveAnyValidationError();
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
