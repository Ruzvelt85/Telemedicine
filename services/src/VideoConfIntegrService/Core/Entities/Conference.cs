using System;
using Newtonsoft.Json;
using Telemedicine.Common.Infrastructure.EntityBase;

namespace Telemedicine.Services.VideoConfIntegrService.Core.Entities
{
    public class Conference : IEntity, IAuditable, ILogicallyDeletable
    {
        [JsonConstructor]
        [Obsolete("Do not use this constructor for creating the entity. It's used in migrations (seeding) and unit tests")]
        public Conference(Guid id, Guid appointmentId, string appointmentTitle, DateTime appointmentStartDate, TimeSpan appointmentDuration,
            string fullExtension, int roomId, string roomUrl, string roomPin, string xmlResponse, DateTime createdOn, DateTime updatedOn, bool isDeleted)
        {
            Id = id;
            AppointmentId = appointmentId;
            AppointmentTitle = appointmentTitle;
            AppointmentStartDate = appointmentStartDate;
            AppointmentDuration = appointmentDuration;
            FullExtension = fullExtension;
            RoomId = roomId;
            RoomUrl = roomUrl;
            RoomPin = roomPin;
            XmlResponse = xmlResponse;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
            IsDeleted = isDeleted;
        }

        public Conference(Guid appointmentId, string appointmentTitle, DateTime appointmentStartDate, TimeSpan appointmentDuration,
            string fullExtension, int roomId, string roomUrl, string roomPin, string xmlResponse)
        {
            AppointmentId = appointmentId;
            AppointmentTitle = appointmentTitle;
            AppointmentStartDate = appointmentStartDate;
            AppointmentDuration = appointmentDuration;
            FullExtension = fullExtension;
            RoomId = roomId;
            RoomUrl = roomUrl;
            RoomPin = roomPin;
            XmlResponse = xmlResponse;
        }

        public Guid Id { get; init; }

        /// <summary> Id of the appointment, for which the video-conference is being created 
        /// </summary>
        public Guid AppointmentId { get; init; }

        /// <summary> Title of the appointment </summary>
        public string AppointmentTitle { get; init; }

        /// <summary> Planned date and time of start of the appointment </summary>
        public DateTime AppointmentStartDate { get; init; }

        /// <summary> Planned duration of the appointment  </summary>
        public TimeSpan AppointmentDuration { get; init; }

        /// <summary>
        /// Generated unique extension, needed for creating a room in external service Vidyo.
        /// Must be unique and consists of 2 parts: fixed tenant's prefix and unique digit postfix.
        /// It's generated on our side
        /// </summary>
        public string FullExtension { get; init; }

        /// <summary> Id of entity 'Room' that was created in external service Vidyo</summary>
        public int RoomId { get; init; }

        /// <summary> Full Url for connecting to the room in Vidyo</summary>
        public string RoomUrl { get; init; }

        /// <summary>
        /// Pin-code for connecting to the room in Vidyo.
        /// If equals NULL, then pin-code is not set.
        /// Pin-code is generated on our side
        /// </summary>
        public string? RoomPin { get; set; }

        /// <summary>
        /// Full xml-response received from Vidyo service for request of creating a room.
        /// </summary>
        public string XmlResponse { get; init; }

        /// <inheritdoc />
        public DateTime CreatedOn { get; init; }

        /// <inheritdoc />
        public DateTime UpdatedOn { get; init; }

        /// <inheritdoc />
        public bool IsDeleted { get; init; }
    }
}
