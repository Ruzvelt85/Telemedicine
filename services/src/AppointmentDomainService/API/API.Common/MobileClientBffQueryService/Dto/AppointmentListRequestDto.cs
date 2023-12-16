using System;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto
{
    public record AppointmentListRequestDto
    {
        public Guid AttendeeId { get; init; }
    }
}
