using FluentValidation;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Constants;

namespace Telemedicine.Services.WebClientBffService.API.Providers.ProviderQueryService.Dto
{
    public class ProviderListFilterRequestDtoValidator : AbstractValidator<ProviderListFilterRequestDto>
    {
        public ProviderListFilterRequestDtoValidator()
        {
            RuleFor(_ => _.HealthCenterIds).SetValidator(new NullableGuidArrayValidator(ValidationRestrictionConstants.MaxHealthCenterQuantity));
            RuleFor(_ => _.Name).MaximumLength(ValidationRestrictionConstants.SearchByNameLength);
        }
    }
}
