using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.User
{
    public class DoctorWriteRepository : WriteRepository<Core.Entities.Doctor>, IDoctorWriteRepository
    {
        /// <inheritdoc />
        public DoctorWriteRepository(HealthCenterStructureDomainServiceDbContext context) : base(context)
        {
        }
    }
}
