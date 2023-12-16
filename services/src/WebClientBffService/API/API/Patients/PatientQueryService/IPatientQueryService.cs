using System;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetAppointmentListByPatientId;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientById;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientList;
using Refit;

namespace Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService
{
    public interface IPatientQueryService
    {
        [Get("/api/users/patients")]
        Task<PatientListResponseDto> GetPatientListAsync(PatientListRequestDto request, CancellationToken cancellationToken = default);

        [Get("/api/users/patients/{id}")]
        Task<PatientByIdResponseDto> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default);

        [Get("/api/users/patients/{id}/appointments")]
        Task<AppointmentListByPatientIdResponseDto> GetAppointmentsByPatientIdAsync(Guid id, AppointmentListByPatientIdRequestDto request, CancellationToken cancellationToken = default);
    }
}
