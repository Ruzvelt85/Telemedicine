using System;
using System.Threading;
using System.Threading.Tasks;
using Refit;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.HealthCentersCommandService.Dto;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.HealthCentersCommandService
{
    public interface IHealthCentersCommandService
    {
        [Post("/api/healthcenters")]
        Task<Guid?> CreateOrUpdateAsync([Body(true)] CreateOrUpdateHealthCenterRequestDto request, CancellationToken cancellationToken = default);
    }
}
