using System;

namespace Telemedicine.Services.VideoConfIntegrService.WebAPI.Services
{
    public record CreateConferenceDto
    {
        public Guid AppointmentId { get; init; }

        public string AppointmentTitle { get; init; }

        public DateTime AppointmentStartDate { get; init; }

        public TimeSpan AppointmentDuration { get; init; }

        public string FullExtension { get; init; }

        public int RoomId { get; init; }

        public string RoomUrl { get; init; }

        public string RoomPin { get; init; }

        public string XmlResponse { get; set; }

#pragma warning disable 8618
        public CreateConferenceDto(Guid appointmentId, string appointmentTitle, DateTime appointmentStartDate, TimeSpan appointmentDuration)
#pragma warning restore 8618
        {
            AppointmentId = appointmentId;
            AppointmentTitle = appointmentTitle;
            AppointmentStartDate = appointmentStartDate;
            AppointmentDuration = appointmentDuration;
        }

#pragma warning disable 8618
        public CreateConferenceDto()
#pragma warning restore 8618
        {

        }
    }
}
