using FluentValidation;
using System;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentList
{
    public class AppointmentListFilterRequestDtoValidator : AbstractValidator<AppointmentListFilterRequestDto>
    {
        public AppointmentListFilterRequestDtoValidator()
        {
            RuleFor(_ => _.AppointmentStatus).IsInEnum();
            RuleFor(_ => _.DateRange).SetValidator(new RangeValidator<DateTime>());
        }
    }
}
