using System;

namespace Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientList
{
    public record PatientListNearestAppointmentInfoResponseDto
    {
        public Guid AppointmentId { get; init; }

        public DateTime StartDate { get; init; }
    }
}
