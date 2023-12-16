using System;

namespace Telemedicine.Services.MobileClientBffService.API.MobileClientQueryService.Dto
{
    public record MoodResponseDto
    {
        public Guid Id { get; init; }

        public MoodMeasureType Measure { get; init; }

        public DateTime ClientDate { get; init; }
    }
}
