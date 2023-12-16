using System;
using System.Threading;
using System.Threading.Tasks;
using Refit;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsCommandService.Dto;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsCommandService
{
    public interface IDoctorsCommandService
    {
        [Post("/api/doctors")]
        Task<Guid?> CreateOrUpdateAsync([Body(true)] CreateOrUpdateDoctorRequestDto request, CancellationToken cancellationToken = default);
    }
}
