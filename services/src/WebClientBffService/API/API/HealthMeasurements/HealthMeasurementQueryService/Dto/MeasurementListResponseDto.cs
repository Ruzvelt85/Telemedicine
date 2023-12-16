using System;
using System.Collections.Generic;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.WebClientBffService.API.HealthMeasurements.HealthMeasurementQueryService.Dto
{
    public record MeasurementListResponseDto
    {
        public static readonly MeasurementListResponseDto Empty = new();

        public PagingResponseDto Paging { get; init; } = new(0);

        public IReadOnlyCollection<BloodPressureResponseDto> BloodPressureItems { get; set; } = Array.Empty<BloodPressureResponseDto>();

        public IReadOnlyCollection<SaturationResponseDto> SaturationItems { get; set; } = Array.Empty<SaturationResponseDto>();

        public IReadOnlyCollection<MoodResponseDto> MoodItems { get; set; } = Array.Empty<MoodResponseDto>();

        public IReadOnlyCollection<PulseRateResponseDto> PulseRateItems { get; set; } = Array.Empty<PulseRateResponseDto>();
    }
}
