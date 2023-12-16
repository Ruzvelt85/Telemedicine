using System;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common
{
    public record MeasurementListFilterRequestDto
    {
        public Guid PatientId { get; init; }

        public Range<DateTime?> DateRange { get; init; } = new();
    }
}
