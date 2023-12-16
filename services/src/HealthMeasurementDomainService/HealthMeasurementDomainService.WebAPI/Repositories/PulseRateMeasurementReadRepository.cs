using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Repositories;
using Telemedicine.Services.HealthMeasurementDomainService.DAL;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Repositories
{
    public class PulseRateMeasurementReadRepository : ReadRepository<PulseRateMeasurement>, IPulseRateMeasurementReadRepository
    {
        public PulseRateMeasurementReadRepository(HealthMeasurementDomainServiceDbContext context) : base(context)
        {
        }
    }
}