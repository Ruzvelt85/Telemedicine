using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto
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
