using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.InterdisciplinaryTeam
{
    public class InterdisciplinaryTeamReadRepository : ReadRepository<Core.Entities.InterdisciplinaryTeam>, IInterdisciplinaryTeamReadRepository
    {
        public InterdisciplinaryTeamReadRepository(HealthCenterStructureDomainServiceDbContext context) : base(context)
        {
        }
    }
}
