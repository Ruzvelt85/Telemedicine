using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.Patterns.Data;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Core.Repositories
{
    public interface IPatientReadRepository : IReadRepository<Patient>
    {
        Task<Patient> SingleOrDefaultWithDeletedAsync(Expression<Func<Patient, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
