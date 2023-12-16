using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentList;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Queries.Appointments
{
    public record GetAppointmentListQuery : IQuery
    {
        public GetAppointmentListQuery(AppointmentListFilterRequestDto filter, PagingRequestDto paging)
        {
            Filter = filter;
            Paging = paging;
        }

        public AppointmentListFilterRequestDto Filter { get; set; }

        public PagingRequestDto Paging { get; set; }
    }
}
