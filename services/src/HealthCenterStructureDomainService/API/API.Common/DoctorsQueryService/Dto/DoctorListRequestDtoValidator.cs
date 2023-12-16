using FluentValidation;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto
{
    public class DoctorListRequestDtoValidator : AbstractValidator<DoctorListRequestDto>
    {
        public DoctorListRequestDtoValidator()
        {
            RuleFor(_ => _.FirstNameSortingType).IsInEnum();
            RuleFor(_ => _.Filter).SetValidator(new DoctorListFilterRequestDtoValidator());
            RuleFor(_ => _.Paging).SetValidator(new PagingRequestDtoValidator());
        }
    }
}
