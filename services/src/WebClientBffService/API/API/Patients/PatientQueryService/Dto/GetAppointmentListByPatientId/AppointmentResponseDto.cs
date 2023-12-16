using System;
using System.Collections.Generic;
using Telemedicine.Services.WebClientBffService.API.Common;

namespace Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetAppointmentListByPatientId
{
    public record AppointmentResponseDto
    {
        public Guid Id { get; init; }

        public string Title { get; init; } = string.Empty;

        public DateTime StartDate { get; init; }

        public TimeSpan Duration { get; init; }

        public AppointmentType Type { get; init; }

        public AppointmentStatus Status { get; init; }

        public int Rating { get; init; }

        public IReadOnlyCollection<AttendeeResponseDto> Attendees { get; init; } = Array.Empty<AttendeeResponseDto>();
    }
}
