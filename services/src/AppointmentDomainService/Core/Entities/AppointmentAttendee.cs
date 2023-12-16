using System;
using Telemedicine.Common.Infrastructure.EntityBase;
using Telemedicine.Common.Infrastructure.Patterns.DomainDrivenDesign;
using Newtonsoft.Json;

namespace Telemedicine.Services.AppointmentDomainService.Core.Entities
{
    [ValueObject]
    public class AppointmentAttendee : IEntity, ILogicallyDeletable
    {
        [JsonConstructor]
        [Obsolete("Do not use this constructor for creating the entity")]
        public AppointmentAttendee(Guid id, Guid appointmentId, Guid userId)
        {
            Id = id;
            AppointmentId = appointmentId;
            UserId = userId;
        }

        public AppointmentAttendee(Guid userId)
        {
            UserId = userId;
        }

        public Guid Id { get; init; }

        public Guid AppointmentId { get; init; }

        public Guid UserId { get; init; }

        /// <inheritdoc />
        public bool IsDeleted { get; init; }
    }
}
