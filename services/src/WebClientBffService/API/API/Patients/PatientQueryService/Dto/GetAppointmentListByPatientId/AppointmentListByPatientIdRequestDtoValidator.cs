using FluentValidation;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetAppointmentListByPatientId
{
    public class AppointmentListByPatientIdRequestDtoValidator : AbstractValidator<AppointmentListByPatientIdRequestDto>
    {
        public AppointmentListByPatientIdRequestDtoValidator()
        {
            RuleFor(_ => _.Filter).SetValidator(new AppointmentListByPatientIdFilterRequestDtoValidator());
            RuleFor(_ => _.Paging).SetValidator(new PagingRequestDtoValidator());
        }
    }
}
