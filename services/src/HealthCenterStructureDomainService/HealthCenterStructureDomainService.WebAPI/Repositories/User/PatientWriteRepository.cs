using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.User
{
    public class PatientWriteRepository : WriteRepository<Core.Entities.Patient>, IPatientWriteRepository
    {
        public PatientWriteRepository(HealthCenterStructureDomainServiceDbContext context) : base(context)
        {
        }
    }
}
