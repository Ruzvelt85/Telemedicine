using System.Collections.Generic;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto
{
    public record AppointmentListResponseDto
    {
        public AppointmentListResponseDto()
        {
            Appointments = new List<AppointmentResponseDto>();
        }

        public IReadOnlyCollection<AppointmentResponseDto> Appointments { get; init; }
    }
}
