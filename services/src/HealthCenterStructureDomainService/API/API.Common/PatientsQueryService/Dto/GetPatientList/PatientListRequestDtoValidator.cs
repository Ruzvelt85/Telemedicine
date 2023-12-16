using FluentValidation;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientList
{
    public class PatientListRequestDtoValidator : AbstractValidator<PatientListRequestDto>
    {
        public PatientListRequestDtoValidator()
        {
            RuleFor(_ => _.FirstNameSortingType).IsInEnum();
            RuleFor(_ => _.Filter).Cascade(CascadeMode.Stop).NotNull().SetValidator(new PatientListFilterRequestDtoValidator()!);
            RuleFor(_ => _.Paging).Cascade(CascadeMode.Stop).NotNull().SetValidator(new PagingRequestDtoValidator()!);
        }
    }
}
