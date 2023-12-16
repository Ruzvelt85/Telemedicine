using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities
{
    public static class QueryableUtilities
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int skip, int take)
        {
            return query.Skip(skip).Take(take);
        }

        public static IQueryable<T> OrderBy<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> keySelector, SortingType sortingType)
        {
            return sortingType == SortingType.Desc
                ? query.OrderByDescending(keySelector)
                : query.OrderBy(keySelector);
        }

        public static async Task<PagedListResponseDto<TResponse>> ToPagedResponseAsync<TEntity, TResponse>(this IQueryable<TEntity> query, IMapper mapper, PagingRequestDto paging, CancellationToken cancellationToken = default)
        {
            var totalCountTask = query.CountAsync(cancellationToken);
            var responsesTask = query.Skip(paging.Skip).Take(paging.Take).ProjectTo<TResponse>(mapper.ConfigurationProvider).ToArrayAsync(cancellationToken);

            await Task.WhenAll(totalCountTask, responsesTask);

            return new PagedListResponseDto<TResponse>
            {
                Items = responsesTask.Result,
                Paging = new PagingResponseDto(totalCountTask.Result)
            };
        }
    }
}
