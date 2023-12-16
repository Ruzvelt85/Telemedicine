using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Telemedicine.Common.Infrastructure.Patterns.Data
{
    [PublicAPI]
    public interface IReadRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetQuery();

        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        Task<ICollection<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        Task<int> CountAsync(CancellationToken cancellationToken);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
