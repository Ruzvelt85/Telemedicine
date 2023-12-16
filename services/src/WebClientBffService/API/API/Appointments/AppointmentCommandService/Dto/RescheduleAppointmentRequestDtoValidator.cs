using System;
using FluentValidation;
using JetBrains.Annotations;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto;

namespace Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentCommandService.Dto
{
    [UsedImplicitly]
    public class RescheduleAppointmentRequestDtoValidator : AbstractValidator<RescheduleAppointmentRequestDto>
    {
        public RescheduleAppointmentRequestDtoValidator()
        {
            RuleFor(_ => _.StartDate).NotEmpty().GreaterThan(DateTime.UtcNow);
            RuleFor(_ => _.Duration).GreaterThanOrEqualTo(AppointmentValidatorConstants.MinDuration).LessThanOrEqualTo(AppointmentValidatorConstants.MaxDuration);
            RuleFor(_ => _.Reason).NotEmpty().MaximumLength(AppointmentValidatorConstants.ReasonMaxLength);
        }
    }
}
