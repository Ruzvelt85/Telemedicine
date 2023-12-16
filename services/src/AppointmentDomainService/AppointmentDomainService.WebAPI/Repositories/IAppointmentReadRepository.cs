using System;
using System.Linq;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Data;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Repositories
{
    public interface IAppointmentReadRepository : IReadRepository<Appointment>
    {
        IQueryable<Appointment> FindWithDeleted(Expression<Func<Appointment, bool>> predicate);
    }
}
