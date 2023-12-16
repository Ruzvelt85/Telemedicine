using System;
using System.Collections.Generic;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto
{
    public record AppointmentListResponseDto
    {
        public IReadOnlyCollection<AppointmentResponseDto> Items { get; init; } = Array.Empty<AppointmentResponseDto>();

        public PagingResponseDto Paging { get; init; } = new(0);
    }
}
