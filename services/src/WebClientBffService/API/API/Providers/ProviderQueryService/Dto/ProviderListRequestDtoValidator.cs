using FluentValidation;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.WebClientBffService.API.Providers.ProviderQueryService.Dto
{
    public class ProviderListRequestDtoValidator : AbstractValidator<ProviderListRequestDto>
    {
        public ProviderListRequestDtoValidator()
        {
            RuleFor(_ => _.FirstNameSortingType).IsInEnum();
            RuleFor(_ => _.Filter).SetValidator(new ProviderListFilterRequestDtoValidator());
            RuleFor(_ => _.Paging).SetValidator(new PagingRequestDtoValidator());
        }
    }
}
