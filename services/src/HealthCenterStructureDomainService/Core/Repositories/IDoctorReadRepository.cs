using System;
using System.Linq;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Data;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Core.Repositories
{
    public interface IDoctorReadRepository : IReadRepository<Doctor>
    {
        IQueryable<Doctor> FindWithDeleted(Expression<Func<Doctor, bool>> predicate);
    }
}
