using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Telemedicine.Common.Infrastructure.Patterns.Query
{
    /// <summary>
    /// Defines a handler for a query
    /// </summary>
    /// <typeparam name="TQuery">The type of query being handled</typeparam>
    /// <typeparam name="TResult">The type of result from the handler</typeparam>
    [PublicAPI]
    public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery
    {
        /// <summary>
        /// Handles a query asynchronously
        /// </summary>
        /// <param name="query">The query</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result from the query</returns>
        Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
    }
}
