using FluentValidation;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto;

namespace Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentCommandService.Dto
{
    public class CancelAppointmentRequestDtoValidator : AbstractValidator<CancelAppointmentRequestDto>
    {
        public CancelAppointmentRequestDtoValidator()
        {
            RuleFor(_ => _.Id).NotEmpty();
            RuleFor(_ => _.Reason).Cascade(CascadeMode.Stop).NotEmpty().MaximumLength(AppointmentValidatorConstants.ReasonMaxLength);
        }
    }
}
