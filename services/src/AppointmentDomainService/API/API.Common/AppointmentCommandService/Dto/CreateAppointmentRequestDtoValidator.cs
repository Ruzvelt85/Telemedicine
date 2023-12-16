using System;
using System.Linq;
using FluentValidation;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto
{
    public class CreateAppointmentRequestDtoValidator : AbstractValidator<CreateAppointmentRequestDto>
    {
        public CreateAppointmentRequestDtoValidator()
        {
            RuleFor(_ => _.Title).Cascade(CascadeMode.Stop).NotEmpty().Length(5, 100);
            RuleFor(_ => _.Description).Length(5, 100);
            RuleFor(_ => _.StartDate).NotEmpty().GreaterThan(DateTime.UtcNow);
            RuleFor(_ => _.Duration).GreaterThanOrEqualTo(AppointmentValidatorConstants.MinDuration).LessThanOrEqualTo(AppointmentValidatorConstants.MaxDuration);
            RuleFor(_ => _.AppointmentType).NotEmpty().IsInEnum();
            RuleFor(_ => _.AttendeeIds).Cascade(CascadeMode.Stop).NotEmpty()
                .Must(_ => _!.Distinct().ToArray().Length > 1).WithMessage("'Attendee Ids' must contain 2 or more unique items.")
                .ForEach(_ => _.Cascade(CascadeMode.Stop).NotEmpty());
            RuleFor(_ => _.CreatorId).NotEmpty();
        }
    }
}
