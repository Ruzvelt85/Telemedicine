using System.Collections.Generic;

namespace Telemedicine.Services.MobileClientBffService.API.MobileClientQueryService.Dto
{
    public record LastChangedDataResponseDto
    {
        public IReadOnlyCollection<AppointmentResponseDto>? Appointments { get; init; }

        public MoodResponseDto? Mood { get; init; }
    }
}
