using System.Collections.Generic;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentList
{
    public record AppointmentListResponseDto
    {
        public IReadOnlyCollection<AppointmentResponseDto>? Items { get; init; }

        public PagingResponseDto? Paging { get; init; }
    }
}
