using System;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto
{
    public record NearestAppointmentsResponseDto
    {
        public Guid AttendeeId { get; init; }

        public NearestAppointmentInfoResponseDto? PreviousAppointmentInfo { get; init; }

        public NearestAppointmentInfoResponseDto? NextAppointmentInfo { get; init; }

        public AppointmentType? NextAppointmentType { get; init; }
    }
}
