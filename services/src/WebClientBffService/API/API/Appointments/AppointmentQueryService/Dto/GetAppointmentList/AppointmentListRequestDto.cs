using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentList
{
    public record AppointmentListRequestDto
    {
        public AppointmentListRequestDto()
        {
            Filter = new AppointmentListFilterRequestDto();
            Paging = new PagingRequestDto();
        }

        public AppointmentListFilterRequestDto Filter { get; init; }

        public PagingRequestDto Paging { get; init; }
    }
}
