using FluentValidation;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto
{
    public class AppointmentListRequestDtoValidator : AbstractValidator<AppointmentListRequestDto>
    {
        public AppointmentListRequestDtoValidator()
        {
            RuleFor(_ => _.AttendeeId).NotEmpty();
        }
    }
}
