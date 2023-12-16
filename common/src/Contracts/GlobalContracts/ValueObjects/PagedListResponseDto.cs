using System;
using System.Collections.Generic;

namespace Telemedicine.Common.Contracts.GlobalContracts.ValueObjects
{
    public record PagedListResponseDto<T>
    {
        public PagingResponseDto Paging { get; init; } = new(0);

        public IReadOnlyCollection<T> Items { get; init; } = Array.Empty<T>();
    }
}
