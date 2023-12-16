using FluentValidation;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsCommandService.Dto
{
    public class CreateOrUpdateDoctorRequestDtoValidator : AbstractValidator<CreateOrUpdateDoctorRequestDto>
    {
        public CreateOrUpdateDoctorRequestDtoValidator()
        {
            RuleFor(_ => _.InnerId).NotEmpty().MaximumLength(55);
            RuleFor(_ => _.LastName).NotEmpty().MaximumLength(50);
            RuleFor(_ => _.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(_ => _.HealthCenterInnerIds).NotEmpty();
            RuleForEach(_ => _.HealthCenterInnerIds).Cascade(CascadeMode.Stop).NotEmpty();
            RuleFor(_ => _.IsActive).NotNull();
        }
    }
}
