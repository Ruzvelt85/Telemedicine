using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.Patterns.Query;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements
{
    public record GetMeasurementListQuery : IQuery
    {
        public HealthMeasurementDomainService.API.Common.MeasurementListFilterRequestDto Filter { get; init; } = new();

        public PagingRequestDto Paging { get; init; } = new();
    }
}
