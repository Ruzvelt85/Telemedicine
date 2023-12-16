using System;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto
{
    public record UpdateAppointmentStatusRequestDto
    {
        public Guid Id { get; init; }

        public string? Reason { get; init; }

        public AppointmentStatus Status { get; init; }
    }
}
