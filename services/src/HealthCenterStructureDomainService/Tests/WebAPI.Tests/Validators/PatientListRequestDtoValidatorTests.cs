using System;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientList;
using Xunit;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.Validators
{
    public class PatientListRequestDtoValidatorTests
    {
        private readonly PatientListRequestDto _defaultModel;
        private readonly PatientListRequestDtoValidator _validator;

        public PatientListRequestDtoValidatorTests()
        {
            _defaultModel = new PatientListRequestDto()
            {
                Paging = new PagingRequestDto(),
                Filter = new PatientListFilterRequestDto(),
            };
            _validator = new PatientListRequestDtoValidator();
        }

        [Fact]
        public async Task Model_Default_ShouldNotHaveValidationErrorFor()
        {
            var result = await _validator.TestValidateAsync(_defaultModel);

            result.ShouldNotHaveValidationErrorFor(_ => _.FirstNameSortingType);
            result.ShouldNotHaveValidationErrorFor(_ => _.Paging);
            result.ShouldNotHaveValidationErrorFor(_ => _.Filter);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task FirstNameSortingType_CorrectValue_ShouldNotHaveValidationErrorFor()
        {
            var model = _defaultModel with { FirstNameSortingType = SortingType.Desc };
            var result = await _validator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.FirstNameSortingType);
            result.ShouldHaveAnyValidationError();
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
                Filter = new PatientListFilterRequestDto()
                {
                    Name = new string('a', 101),
                    DoctorId = Guid.NewGuid(),
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
        public async Task Filter_NestedProperty_Name_ShouldHaveValidationErrorFor()
        {
            var model = _defaultModel with
            {
                Filter = new PatientListFilterRequestDto
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

            result.ShouldHaveValidationErrorFor(_ => _.Paging!.Take);
            result.ShouldHaveAnyValidationError();
        }
    }
}
