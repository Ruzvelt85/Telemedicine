using System;
using System.Collections.Generic;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.WebClientBffService.API.Common;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements
{
    public record MeasurementListResponse
    {
        public static readonly MeasurementListResponse Empty = new();

        public PagingResponseDto Paging { get; init; } = new(0);

        public IReadOnlyCollection<IHasClientDate> Items { get; init; } = Array.Empty<IHasClientDate>();
    }
}
