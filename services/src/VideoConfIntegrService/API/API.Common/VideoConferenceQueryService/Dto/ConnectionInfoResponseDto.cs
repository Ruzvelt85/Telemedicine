using System;

namespace Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceQueryService.Dto
{
    public record ConnectionInfoResponseDto
    {
        public Guid AppointmentId { get; init; }

        public int RoomId { get; init; }

        public string Host { get; set; } = string.Empty;

        public string RoomKey { get; set; } = string.Empty;

        public string? RoomPin { get; init; }
    }
}
