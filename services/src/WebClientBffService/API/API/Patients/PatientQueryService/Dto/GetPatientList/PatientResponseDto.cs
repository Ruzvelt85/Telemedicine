using System;
using Telemedicine.Services.WebClientBffService.API.Common;

namespace Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientList
{
    public record PatientResponseDto
    {
        public Guid? Id { get; init; }

        public string? FirstName { get; init; }

        public string? LastName { get; init; }

        public string? PhoneNumber { get; init; }

        public PatientListNearestAppointmentInfoResponseDto? PreviousAppointmentInfo { get; init; }

        public PatientListNearestAppointmentInfoResponseDto? NextAppointmentInfo { get; init; }

        public AppointmentType? NextAppointmentType { get; init; }
    }
}
