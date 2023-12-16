using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.EntityBase;
using Telemedicine.Common.Infrastructure.Patterns.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories
{
    /// <summary>
    /// Base implementation of pattern Repository for queries of reading data
    /// </summary>
    public abstract class ReadRepository<TEntity> : IReadRepository<TEntity>
        where TEntity : class, IEntity
    {
        protected readonly EfCoreDbContext Context;
        protected readonly ILogger Logger;

        protected ReadRepository(EfCoreDbContext context)
        {
            Context = context;
            Logger = Log.ForContext(this.GetType());
            Logger.Verbose("ReadRepository for {EntityName} entity type created", typeof(TEntity).Name);
        }

        protected IQueryable<TEntity> DbSet
        {
            get
            {
                var dbSet = Context.Set<TEntity>();

                if (TrackChanges)
                {
                    return dbSet;
                }

                return dbSet.AsNoTracking();
            }
        }

        /// <summary>
        /// Is need  track changes by Entity framework
        /// </summary>
        protected virtual bool TrackChanges => false;

        public virtual IQueryable<TEntity> GetQuery() => DbSet.AsQueryable();

        public virtual Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) => DbSet.ToListAsync(cancellationToken);

        public virtual IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate) => DbSet.Where(predicate);

        public virtual async Task<ICollection<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
            await DbSet.Where(predicate).ToListAsync(cancellationToken);

        public virtual Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public virtual Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
            DbSet.SingleOrDefaultAsync(predicate, cancellationToken);

        public virtual Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
            DbSet.FirstOrDefaultAsync(predicate, cancellationToken);

        public virtual Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
            DbSet.AnyAsync(predicate, cancellationToken);

        public virtual Task<int> CountAsync(CancellationToken cancellationToken = default) => DbSet.CountAsync(cancellationToken);

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
            DbSet.CountAsync(predicate, cancellationToken);
    }
}
