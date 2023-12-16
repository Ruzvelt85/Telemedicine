using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common
{
    public record GetMeasurementListRequestDto
    {
        public MeasurementListFilterRequestDto Filter { get; init; } = new();

        public PagingRequestDto Paging { get; init; } = new();
    }
}
