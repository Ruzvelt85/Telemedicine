using System;
using FluentValidation;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetAppointmentListByPatientId
{
    public class AppointmentListByPatientIdFilterRequestDtoValidator : AbstractValidator<AppointmentListByPatientIdFilterRequestDto>
    {
        public AppointmentListByPatientIdFilterRequestDtoValidator()
        {
            RuleFor(_ => _.AppointmentStatus).IsInEnum();
            RuleFor(_ => _.DateRange).SetValidator(new RangeValidator<DateTime>());
        }
    }
}
