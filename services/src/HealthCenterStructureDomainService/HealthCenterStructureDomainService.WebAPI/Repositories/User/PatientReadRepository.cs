using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;
using Microsoft.EntityFrameworkCore;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.User
{
    public class PatientReadRepository : ReadRepository<Core.Entities.Patient>, IPatientReadRepository
    {
        public PatientReadRepository(HealthCenterStructureDomainServiceDbContext context) : base(context)
        {
        }

        public Task<Core.Entities.Patient> SingleOrDefaultWithDeletedAsync(Expression<Func<Core.Entities.Patient, bool>> predicate, CancellationToken cancellationToken = default) =>
            DbSet.IgnoreQueryFilters().SingleOrDefaultAsync(predicate, cancellationToken);
    }
}
