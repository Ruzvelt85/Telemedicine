using Telemedicine.Common.Infrastructure.Patterns.Data;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Repositories
{
    public interface IAppointmentWriteRepository : IWriteRepository<Appointment>
    {
    }
}
