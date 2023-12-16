using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.HealthCenter
{
    public class HealthCenterWriteRepository : WriteRepository<Core.Entities.HealthCenter>, IHealthCenterWriteRepository
    {
        /// <inheritdoc />
        public HealthCenterWriteRepository(HealthCenterStructureDomainServiceDbContext context) : base(context)
        {
        }
    }
}
