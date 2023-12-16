using FluentValidation;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto
{
    public class UpdateAppointmentStatusRequestDtoValidator : AbstractValidator<UpdateAppointmentStatusRequestDto>
    {
        public UpdateAppointmentStatusRequestDtoValidator()
        {
            RuleFor(_ => _.Id).NotEmpty();
            RuleFor(_ => _.Reason).Cascade(CascadeMode.Stop)
                .NotEmpty().When(_ => _.Status == AppointmentStatus.Cancelled, ApplyConditionTo.CurrentValidator)
                .MaximumLength(AppointmentValidatorConstants.ReasonMaxLength);
            RuleFor(_ => _.Status).NotEmpty().IsInEnum();
        }
    }
}
