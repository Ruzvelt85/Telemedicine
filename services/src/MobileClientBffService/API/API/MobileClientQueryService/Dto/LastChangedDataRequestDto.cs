using System;

namespace Telemedicine.Services.MobileClientBffService.API.MobileClientQueryService.Dto
{
    public record LastChangedDataRequestDto
    {
        public DateTime? AppointmentsLastUpdate { get; init; }

        public DateTime? MoodLastUpdate { get; init; }
    }
}
