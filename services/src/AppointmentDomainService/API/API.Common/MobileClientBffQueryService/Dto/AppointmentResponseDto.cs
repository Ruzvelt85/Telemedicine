using System;
using System.Collections.Generic;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto
{
    public record AppointmentResponseDto
    {
        public AppointmentResponseDto(Guid id, string title, DateTime startDate, TimeSpan duration, AppointmentState state, AppointmentType type, DateTime updatedOn, bool isDeleted)
        {
            Id = id;
            Title = title;
            StartDate = startDate;
            Duration = duration;
            State = state;
            Type = type;
            UpdatedOn = updatedOn;
            IsDeleted = isDeleted;
            Attendees = new List<Guid>();
        }

        public Guid Id { get; init; }

        public string Title { get; init; }

        public DateTime StartDate { get; init; }

        public TimeSpan Duration { get; init; }

        public AppointmentState State { get; init; }

        public AppointmentType Type { get; init; }

        public DateTime UpdatedOn { get; init; }

        public bool IsDeleted { get; init; }

        public IReadOnlyCollection<Guid> Attendees { get; init; }
    }
}