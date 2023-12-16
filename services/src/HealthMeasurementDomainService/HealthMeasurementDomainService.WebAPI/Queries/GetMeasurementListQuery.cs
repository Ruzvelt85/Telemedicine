using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Queries
{
    public class GetMeasurementListQuery : IQuery
    {
        public MeasurementListFilterRequestDto Filter { get; init; } = new();

        public PagingRequestDto Paging { get; init; } = new();
    }
}
