using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Xunit;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.Validators
{
    public class DoctorListFilterRequestValidatorTests
    {
        private readonly DoctorListFilterRequestDto _defaultModel;
        private readonly DoctorListFilterRequestDtoValidator _dtoValidator;

        public DoctorListFilterRequestValidatorTests()
        {
            _defaultModel = new DoctorListFilterRequestDto();
            _dtoValidator = new DoctorListFilterRequestDtoValidator();
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
    }
}
