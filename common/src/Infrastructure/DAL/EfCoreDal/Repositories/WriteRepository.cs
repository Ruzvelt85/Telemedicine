using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.EntityBase;
using Telemedicine.Common.Infrastructure.Patterns.Data;
using Serilog;

namespace Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories
{
    /// <summary>
    /// Base implementation of pattern Repository for queries of writing data
    /// </summary>
    public abstract class WriteRepository<TEntity> : IWriteRepository<TEntity>
        where TEntity : class, IEntity
    {
        protected readonly EfCoreDbContext Context;
        protected readonly ILogger Logger;

        protected WriteRepository(EfCoreDbContext context)
        {
            Context = context;

            Logger = Log.ForContext(this.GetType());
            Logger.Verbose("WriteRepository for {EntityName} entity type created", typeof(TEntity).Name);
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
            (await Context.Set<TEntity>().AddAsync(entity, cancellationToken)).Entity;

        public virtual Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) =>
            Context.Set<TEntity>().AddRangeAsync(entities, cancellationToken);

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) =>
            await Task.FromResult(Context.Set<TEntity>().Update(entity).Entity);

        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            Context.Set<TEntity>().UpdateRange(entities);
            await Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Context.Set<TEntity>().Remove(entity);
            await Task.CompletedTask;
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            Context.Set<TEntity>().RemoveRange(entities);
            await Task.CompletedTask;
        }
    }
}
