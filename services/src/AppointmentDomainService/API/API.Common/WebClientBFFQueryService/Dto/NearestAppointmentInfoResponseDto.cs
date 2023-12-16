using System;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto
{
    public record NearestAppointmentInfoResponseDto
    {
        public Guid AppointmentId { get; init; }

        public DateTime StartDate { get; init; }
    }
}
