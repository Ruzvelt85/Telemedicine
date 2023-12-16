using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.QueryHandlers.Setup
{
    public class AppointmentTestReadRepository : ReadRepository<Appointment>
    {
        public AppointmentTestReadRepository(AppointmentTestDbContext context) : base(context)
        {
        }
    }
}
