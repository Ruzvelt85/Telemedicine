using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.WebClientBffService.API.HealthMeasurements.HealthMeasurementQueryService.Dto
{
    public record GetMeasurementListRequestDto
    {
        public MeasurementListFilterRequestDto Filter { get; init; } = new();

        public PagingRequestDto Paging { get; init; } = new();
    }
}
