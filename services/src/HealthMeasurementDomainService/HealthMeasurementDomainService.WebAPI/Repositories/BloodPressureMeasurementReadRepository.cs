using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Repositories;
using Telemedicine.Services.HealthMeasurementDomainService.DAL;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Repositories
{
    public class BloodPressureMeasurementReadRepository : ReadRepository<BloodPressureMeasurement>, IBloodPressureMeasurementReadRepository
    {
        public BloodPressureMeasurementReadRepository(HealthMeasurementDomainServiceDbContext context) : base(context)
        {
        }
    }
}