using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Queries.WebClient
{
    public record GetAppointmentListQuery : IQuery
    {
        public GetAppointmentListQuery(AppointmentListFilterRequestDto filter, PagingRequestDto paging)
        {
            Filter = filter;
            Paging = paging;
        }

        public AppointmentListFilterRequestDto Filter { get; init; }

        public PagingRequestDto Paging { get; init; }
    }
}
