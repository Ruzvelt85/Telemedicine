using System;
using System.Linq;
using FluentValidation;

namespace Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentCommandService.Dto
{
    public class CreateAppointmentRequestDtoValidator : AbstractValidator<CreateAppointmentRequestDto>
    {
        private static readonly TimeSpan _appointmentMinDuration = TimeSpan.FromMinutes(15);

        private static readonly TimeSpan _appointmentMaxDuration = TimeSpan.FromHours(3);

        public CreateAppointmentRequestDtoValidator()
        {
            RuleFor(_ => _.Title).Cascade(CascadeMode.Stop).NotEmpty().Length(5, 100);
            RuleFor(_ => _.Description).Length(5, 100);
            RuleFor(_ => _.StartDate).NotEmpty().GreaterThan(DateTime.UtcNow);
            RuleFor(_ => _.Duration).GreaterThanOrEqualTo(_appointmentMinDuration).LessThanOrEqualTo(_appointmentMaxDuration);
            RuleFor(_ => _.AppointmentType).NotEmpty().IsInEnum();
            RuleFor(_ => _.AttendeeIds).Cascade(CascadeMode.Stop).NotEmpty()
                .Must(_ => _!.Distinct().ToArray().Length > 1).WithMessage("'Attendee Ids' must contain 2 or more unique items.")
                .ForEach(_ => _.Cascade(CascadeMode.Stop).NotEmpty());
        }
    }
}
