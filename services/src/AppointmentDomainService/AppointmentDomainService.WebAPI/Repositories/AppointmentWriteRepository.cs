using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Repositories
{
    public class AppointmentWriteRepository : WriteRepository<Appointment>, IAppointmentWriteRepository
    {
        /// <inheritdoc />
        public AppointmentWriteRepository(AppointmentDomainServiceDbContext context) : base(context)
        {
        }
    }
}
