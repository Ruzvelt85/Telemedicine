using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto
{
    public class AppointmentListFilterRequestDtoValidator : AbstractValidator<AppointmentListFilterRequestDto>
    {
        public AppointmentListFilterRequestDtoValidator()
        {
            RuleFor(_ => _.AttendeeId).NotEmpty();
            RuleFor(_ => _.DateRange).SetValidator(new RangeValidator<DateTime>());
            RuleFor(_ => _.AppointmentStates).Must(IsNullOrEmptyOrAllOrUnique);
            RuleForEach(_ => _.AppointmentStates)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .IsInEnum();
        }

        private static bool IsNullOrEmptyOrAllOrUnique(IReadOnlyCollection<AppointmentState>? appointmentStates)
        {
            if (appointmentStates is null || !appointmentStates.Any())
            {
                return true;
            }

            if (appointmentStates.Contains(AppointmentState.All) && appointmentStates.Count > 1)
            {
                return false;
            }

            return appointmentStates.Count == appointmentStates
                .Distinct()
                .Count();
        }
    }
}
