using FluentValidation;

namespace Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientList
{
    public class PatientListFilterRequestDtoValidator : AbstractValidator<PatientListFilterRequestDto>
    {
        public PatientListFilterRequestDtoValidator()
        {
            RuleFor(_ => _.Name).MaximumLength(101); // FirstName + space + LastName
            RuleFor(_ => _.HealthCenterStructureFilter).IsInEnum();
        }
    }
}
