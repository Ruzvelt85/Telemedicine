using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.User
{
    public class DoctorReadRepository : ReadRepository<Core.Entities.Doctor>, IDoctorReadRepository
    {
        /// <inheritdoc />
        public DoctorReadRepository(HealthCenterStructureDomainServiceDbContext context) : base(context)
        {
        }

        public IQueryable<Core.Entities.Doctor> FindWithDeleted(Expression<Func<Core.Entities.Doctor, bool>> predicate) => DbSet.IgnoreQueryFilters().Where(predicate);
    }
}
