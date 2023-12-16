using FluentValidation;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Constants;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto
{
    public class DoctorListFilterRequestDtoValidator : AbstractValidator<DoctorListFilterRequestDto>
    {
        public DoctorListFilterRequestDtoValidator()
        {
            RuleFor(_ => _.HealthCenterIds).SetValidator(new NullableGuidArrayValidator(ValidationRestrictionConstants.MaxHealthCenterQuantity));
            RuleFor(_ => _.Name).MaximumLength(ValidationRestrictionConstants.SearchByNameLength);
        }
    }
}
