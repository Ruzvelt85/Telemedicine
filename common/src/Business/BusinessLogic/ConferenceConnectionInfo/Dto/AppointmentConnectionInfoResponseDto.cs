using System;

namespace Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Dto
{
    public record AppointmentConnectionInfoResponseDto
    {
        public Guid Id { get; init; }

        public int RoomId { get; init; }

        public string Host { get; init; } = string.Empty;

        public string RoomKey { get; init; } = string.Empty;

        public string? RoomPin { get; init; }
    }
}
