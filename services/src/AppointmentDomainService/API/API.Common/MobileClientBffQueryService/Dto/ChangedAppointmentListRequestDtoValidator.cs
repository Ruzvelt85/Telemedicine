using FluentValidation;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto
{
    public class ChangedAppointmentListRequestDtoValidator : AbstractValidator<ChangedAppointmentListRequestDto>
    {
        public ChangedAppointmentListRequestDtoValidator()
        {
            RuleFor(_ => _.AttendeeId).NotEmpty();
            RuleFor(_ => _.LastUpdate).NotEmpty();
        }
    }
}
