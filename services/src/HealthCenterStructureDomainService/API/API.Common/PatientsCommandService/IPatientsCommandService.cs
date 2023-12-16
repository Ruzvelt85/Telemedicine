using System;
using System.Threading;
using System.Threading.Tasks;
using Refit;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsCommandService.Dto;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsCommandService
{
    public interface IPatientsCommandService
    {
        [Post("/api/patients")]
        Task<Guid?> CreateOrUpdateAsync([Body(true)] CreateOrUpdatePatientRequestDto request, CancellationToken cancellationToken = default);
    }
}
