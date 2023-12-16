using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.User
{
    public class UserReadRepository : ReadRepository<Core.Entities.User>, IUserReadRepository
    {
        public UserReadRepository(HealthCenterStructureDomainServiceDbContext context) : base(context)
        {
        }
    }
}
