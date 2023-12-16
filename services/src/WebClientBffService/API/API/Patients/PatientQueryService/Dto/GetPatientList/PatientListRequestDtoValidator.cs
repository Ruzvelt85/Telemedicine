using FluentValidation;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientList
{
    public class PatientListRequestDtoValidator : AbstractValidator<PatientListRequestDto>
    {
        public PatientListRequestDtoValidator()
        {
            RuleFor(_ => _.FirstNameSortingType).IsInEnum();
            RuleFor(_ => _.Filter).Cascade(CascadeMode.Stop).NotNull().SetValidator(new PatientListFilterRequestDtoValidator()!);
            RuleFor(_ => _.Paging).SetValidator(new PagingRequestDtoValidator());
        }
    }
}
