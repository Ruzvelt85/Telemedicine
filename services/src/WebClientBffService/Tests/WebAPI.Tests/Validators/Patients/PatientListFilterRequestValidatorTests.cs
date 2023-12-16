using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Telemedicine.Services.WebClientBffService.API.Common;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientList;
using Xunit;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Validators.Patients
{
    public class PatientListFilterRequestValidatorTests
    {
        private readonly PatientListFilterRequestDto _defaultModel;
        private readonly PatientListFilterRequestDtoValidator _dtoValidator;

        public PatientListFilterRequestValidatorTests()
        {
            _defaultModel = new PatientListFilterRequestDto();
            _dtoValidator = new PatientListFilterRequestDtoValidator();
        }


        [Fact]
        public async Task Name_Default_ShouldNotHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldNotHaveValidationErrorFor(_ => _.Name);
        }

        [Fact]
        public async Task Name_Null_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Name = null };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Name);
        }

        [Fact]
        public async Task Name_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Name = "Abc-De Efg's" };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Name);
        }

        [Fact]
        public async Task Name_WithCorrectMaxLength_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { Name = new string('a', 101) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.Name);
        }

        [Fact]
        public async Task Name_WithLengthMoreThanMaxLength_ShouldHaveValidationError()
        {
            var model = _defaultModel with { Name = new string('a', 102) };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.Name);
        }

        [Fact]
        public async Task HealthCenterStructureFilter_Default_ShouldNotHaveValidationError()
        {
            var result = await _dtoValidator.TestValidateAsync(_defaultModel);

            result.ShouldNotHaveValidationErrorFor(_ => _.HealthCenterStructureFilter);
        }

        [Fact]
        public async Task HealthCenterStructureFilter_Correct_ShouldNotHaveValidationError()
        {
            var model = _defaultModel with { HealthCenterStructureFilter = HealthCenterStructureFilterType.HealthCenter };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveValidationErrorFor(_ => _.HealthCenterStructureFilter);
        }

        [Fact]
        public async Task HealthCenterStructureFilter_Incorrect_ShouldHaveValidationError()
        {
            var model = _defaultModel with { HealthCenterStructureFilter = (HealthCenterStructureFilterType)11 };
            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.HealthCenterStructureFilter);
        }
    }
}
