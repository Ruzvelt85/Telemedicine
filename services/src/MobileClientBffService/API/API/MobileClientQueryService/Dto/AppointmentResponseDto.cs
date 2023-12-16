using System;
using System.Collections.Generic;

namespace Telemedicine.Services.MobileClientBffService.API.MobileClientQueryService.Dto
{
    public record AppointmentResponseDto
    {
        public AppointmentResponseDto(Guid id, string title, DateTime startDate, TimeSpan duration, AppointmentStatus status, AppointmentType type, DateTime updatedOn, bool isDeleted)
        {
            Id = id;
            Title = title;
            StartDate = startDate;
            Duration = duration;
            Status = status;
            Type = type;
            UpdatedOn = updatedOn;
            IsDeleted = isDeleted;
            Attendees = new List<AttendeeResponseDto>();
        }

        public Guid Id { get; init; }

        public string Title { get; init; }

        public DateTime StartDate { get; init; }

        public TimeSpan Duration { get; init; }

        public AppointmentStatus Status { get; init; }

        public AppointmentType Type { get; init; }

        public DateTime UpdatedOn { get; init; }

        public bool IsDeleted { get; init; }

        public IReadOnlyCollection<AttendeeResponseDto> Attendees { get; init; }
    }
}
