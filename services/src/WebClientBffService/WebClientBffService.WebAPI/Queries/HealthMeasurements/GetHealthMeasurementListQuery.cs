using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.WebClientBffService.API.HealthMeasurements.HealthMeasurementQueryService.Dto;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements
{
    public record GetHealthMeasurementListQuery : IQuery
    {
        public MeasurementListFilterRequestDto Filter { get; set; }

        public PagingRequestDto Paging { get; set; }

        public GetHealthMeasurementListQuery(MeasurementListFilterRequestDto filter, PagingRequestDto paging)
        {
            Filter = filter;
            Paging = paging;
        }
    }
}
