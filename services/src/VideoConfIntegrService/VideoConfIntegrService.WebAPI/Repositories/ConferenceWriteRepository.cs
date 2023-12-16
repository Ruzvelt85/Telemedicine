using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;
using Telemedicine.Services.VideoConfIntegrService.Core.Entities;
using Telemedicine.Services.VideoConfIntegrService.Core.Repositories;
using Telemedicine.Services.VideoConfIntegrService.DAL;

namespace Telemedicine.Services.VideoConfIntegrService.WebAPI.Repositories
{
    public class ConferenceWriteRepository : WriteRepository<Conference>, IConferenceWriteRepository
    {
        public ConferenceWriteRepository(VideoConferenceIntegrationServiceDbContext context) : base(context)
        {
        }
    }
}
