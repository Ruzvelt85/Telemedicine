using System;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.WebClientBffService.API.Common;

namespace Telemedicine.Services.WebClientBffService.API.HealthMeasurements.HealthMeasurementQueryService.Dto
{
    public record MeasurementListFilterRequestDto
    {
        public Range<DateTime?> DateRange { get; init; } = new();

        public Guid PatientId { get; init; }

        public MeasurementType MeasurementType { get; init; } = MeasurementType.All;
    }
}
