using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Repositories;
using Telemedicine.Services.HealthMeasurementDomainService.DAL;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Repositories
{
    public class MoodMeasurementReadRepository : ReadRepository<MoodMeasurement>, IMoodMeasurementReadRepository
    {
        public MoodMeasurementReadRepository(HealthMeasurementDomainServiceDbContext context) : base(context)
        {
        }
    }
}
