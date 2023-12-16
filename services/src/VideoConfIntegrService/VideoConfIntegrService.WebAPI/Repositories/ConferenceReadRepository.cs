using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;
using Telemedicine.Services.VideoConfIntegrService.Core.Entities;
using Telemedicine.Services.VideoConfIntegrService.Core.Repositories;
using Telemedicine.Services.VideoConfIntegrService.DAL;

namespace Telemedicine.Services.VideoConfIntegrService.WebAPI.Repositories
{
    public class ConferenceReadRepository : ReadRepository<Conference>, IConferenceReadRepository
    {
        public ConferenceReadRepository(VideoConferenceIntegrationServiceDbContext context) : base(context)
        {
        }
    }
}
