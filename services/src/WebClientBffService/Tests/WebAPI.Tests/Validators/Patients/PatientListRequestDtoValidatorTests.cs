using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.WebClientBffService.API.Common;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientList;
using Xunit;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Validators.Patients
{
    public class PatientListRequestDtoValidatorTests
    {
        private readonly PatientListRequestDto _defaultRequestDto;
        private readonly PatientListRequestDtoValidator _validator;

        public PatientListRequestDtoValidatorTests()
        {
            _defaultRequestDto = new PatientListRequestDto
            {
                Paging = new PagingRequestDto(),
                Filter = new PatientListFilterRequestDto(),
                FirstNameSortingType = SortingType.Asc
            };
            _validator = new PatientListRequestDtoValidator();
        }

        [Fact]
        public async Task RequestDto_Default_ShouldNotHaveValidationErrorFor()
        {
            var result = await _validator.TestValidateAsync(_defaultRequestDto);

            result.ShouldNotHaveValidationErrorFor(_ => _.FirstNameSortingType);
            result.ShouldNotHaveValidationErrorFor(_ => _.Paging);
            result.ShouldNotHaveValidationErrorFor(_ => _.Filter);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task RequestDto_FirstNameSortingType_CorrectValue_ShouldNotHaveValidationErrorFor()
        {
            var model = _defaultRequestDto with { FirstNameSortingType = SortingType.Desc };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.FirstNameSortingType);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task RequestDto_FirstNameSortingType_IncorrectValue_ShouldHaveValidationErrorFor()
        {
            var model = _defaultRequestDto with { FirstNameSortingType = (SortingType)11 };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.FirstNameSortingType);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task RequestDto_Correct_ShouldNotHaveValidationErrorFor()
        {
            var model = _defaultRequestDto with
            {
                Filter = new PatientListFilterRequestDto()
                {
                    Name = new string('a', 101),
                    HealthCenterStructureFilter = HealthCenterStructureFilterType.HealthCenter
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
        public async Task RequestDto_Filter_NestedProperty_Name_ShouldHaveValidationErrorFor()
        {
            var model = _defaultRequestDto with
            {
                Filter = new PatientListFilterRequestDto()
                {
                    Name = new string('a', 102)
                }
            };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Filter!.Name);
            result.ShouldHaveAnyValidationError();
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
