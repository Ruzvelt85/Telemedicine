using FluentValidation;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentList
{
    public class AppointmentListRequestDtoValidator : AbstractValidator<AppointmentListRequestDto>
    {
        public AppointmentListRequestDtoValidator()
        {
            RuleFor(_ => _.Filter).SetValidator(new AppointmentListFilterRequestDtoValidator());
            RuleFor(_ => _.Paging).SetValidator(new PagingRequestDtoValidator());
        }
    }
}
