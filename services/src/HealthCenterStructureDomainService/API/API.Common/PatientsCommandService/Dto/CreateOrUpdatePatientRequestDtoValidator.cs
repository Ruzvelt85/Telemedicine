using System;
using FluentValidation;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsCommandService.Dto
{
    public class CreateOrUpdatePatientRequestDtoValidator : AbstractValidator<CreateOrUpdatePatientRequestDto>
    {
        public CreateOrUpdatePatientRequestDtoValidator()
        {
            RuleFor(_ => _.InnerId).NotEmpty().MaximumLength(55);
            RuleFor(_ => _.LastName).NotEmpty().MaximumLength(50);
            RuleFor(_ => _.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(_ => _.PhoneNumber).MaximumLength(32);
            RuleFor(_ => _.HealthCenterInnerId).NotEmpty().MaximumLength(36);
            RuleFor(_ => _.PrimaryCareProviderInnerId).MaximumLength(36);
            RuleFor(_ => _.BirthDate).GreaterThan(DateTime.MinValue).When(x => x.BirthDate.HasValue);
            RuleFor(_ => _.IsActive).NotNull();
        }
    }
}
