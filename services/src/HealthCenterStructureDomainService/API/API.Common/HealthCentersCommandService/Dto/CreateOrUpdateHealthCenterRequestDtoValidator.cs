using FluentValidation;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.HealthCentersCommandService.Dto
{
    public class CreateOrUpdateHealthCenterRequestDtoValidator : AbstractValidator<CreateOrUpdateHealthCenterRequestDto>
    {
        public CreateOrUpdateHealthCenterRequestDtoValidator()
        {
            RuleFor(_ => _.InnerId).NotEmpty().MaximumLength(55);
            RuleFor(_ => _.Name).NotEmpty().MaximumLength(64);
            RuleFor(_ => _.UsaState).NotEmpty().MaximumLength(20);
            RuleFor(_ => _.IsActive).NotNull();
        }
    }
}
