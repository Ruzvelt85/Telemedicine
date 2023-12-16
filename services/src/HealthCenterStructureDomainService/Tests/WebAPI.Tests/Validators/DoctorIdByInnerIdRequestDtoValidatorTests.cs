using System.Threading.Tasks;
using Xunit;
using FluentValidation.TestHelper;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Constants;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.Validators
{
    public class DoctorIdByInnerIdRequestDtoValidatorTests
    {
        private readonly DoctorByInnerIdRequestDtoValidator _dtoValidator;

        public DoctorIdByInnerIdRequestDtoValidatorTests()
        {
            _dtoValidator = new DoctorByInnerIdRequestDtoValidator();
        }

        [Fact]
        public async Task InnerId_Empty_ShouldHaveValidationError()
        {
            var model = new DoctorByInnerIdRequestDto(string.Empty);

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.InnerId);
        }

        [Fact]
        public async Task InnerId_LengthMoreThanMaxLength_ShouldHaveValidationError()
        {
            var model = new DoctorByInnerIdRequestDto(new string('a', FieldLengthConstants.InnerIdLength + 1));

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldHaveValidationErrorFor(_ => _.InnerId);
        }

        [Fact]
        public async Task InnerId_Correct_ShouldNotHaveValidationError()
        {
            var model = new DoctorByInnerIdRequestDto("innerId");

            var result = await _dtoValidator.TestValidateAsync(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
