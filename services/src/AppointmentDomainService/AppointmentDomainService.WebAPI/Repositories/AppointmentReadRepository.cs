using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Repositories
{
    public class AppointmentReadRepository : ReadRepository<Appointment>, IAppointmentReadRepository
    {
        public AppointmentReadRepository(AppointmentDomainServiceDbContext context) : base(context)
        {
        }

        public IQueryable<Appointment> FindWithDeleted(Expression<Func<Appointment, bool>> predicate) =>
            DbSet.IgnoreQueryFilters().Where(predicate);

        ///<inheritdoc />
        public override Task<Appointment> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            DbSet.Include(a => a.Attendees).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
