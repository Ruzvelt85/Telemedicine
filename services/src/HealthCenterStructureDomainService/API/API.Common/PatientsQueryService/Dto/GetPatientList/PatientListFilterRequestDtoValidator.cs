using FluentValidation;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientList
{
    public class PatientListFilterRequestDtoValidator : AbstractValidator<PatientListFilterRequestDto>
    {
        public PatientListFilterRequestDtoValidator()
        {
            RuleFor(_ => _.Name).MaximumLength(101); // FirstName + space + LastName
            RuleFor(_ => _.DoctorId).NotEmpty();
            RuleFor(_ => _.HealthCenterStructureFilter).IsInEnum();
        }
    }
}
