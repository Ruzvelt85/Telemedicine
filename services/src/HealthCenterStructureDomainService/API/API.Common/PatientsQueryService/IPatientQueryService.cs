using System;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientById;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientList;
using Refit;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService
{
    public interface IPatientsQueryService
    {
        [Get("/api/patients")]
        Task<PatientListResponseDto> GetPatientListAsync(PatientListRequestDto request, CancellationToken cancellationToken = default);

        /// <exception cref="EntityNotFoundException">Thrown when there is no patient with specified id</exception>
        [Get("/api/patients/{id}")]
        Task<PatientByIdResponseDto> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
