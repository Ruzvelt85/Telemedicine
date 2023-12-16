using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Repositories;
using Telemedicine.Services.HealthMeasurementDomainService.DAL;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Repositories
{
    public class MoodMeasurementWriteRepository : WriteRepository<MoodMeasurement>, IMoodMeasurementWriteRepository
    {
        public MoodMeasurementWriteRepository(HealthMeasurementDomainServiceDbContext context) : base(context)
        {
        }
    }
}
