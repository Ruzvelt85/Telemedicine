using System;
using Newtonsoft.Json;
using Telemedicine.Common.Infrastructure.EntityBase;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Enums;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities
{
    public class User : IEntity, IInnerIdSystem, IPersonName, IAuditable, ILogicallyDeletable, IOptimisticLock
    {
        [JsonConstructor]
        [Obsolete("Do not use this constructor for creating the entity")]
        protected User(Guid id, string innerId, string firstName, string lastName, string? phoneNumber, UserType type, DateTime createdOn, DateTime updatedOn, bool isDeleted)
        {
            Id = id;
            InnerId = innerId;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Type = type;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
            IsDeleted = isDeleted;
        }

        protected User(string innerId, string firstName, string lastName, string? phoneNumber, UserType type, bool isDeleted)
        {
            InnerId = innerId;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Type = type;
            IsDeleted = isDeleted; // TODO: JD-1282 - Remove setting of this property while creating the entity
        }

        // TODO: Remove this constructor for Update Command, check InfrastructureTest after changes
        [Obsolete("Do not use this constructor for creating the entity; it's used by Automapper while updating an entity")]
#pragma warning disable 8618
        public User(string firstName, string lastName, string? phoneNumber, UserType type, bool isDeleted)
#pragma warning restore 8618
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Type = type;
            IsDeleted = isDeleted;
        }

        public Guid Id { get; set; }

        public string InnerId { get; init; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? PhoneNumber { get; init; }

        public UserType Type { get; init; }

        /// <inheritdoc />
        public DateTime CreatedOn { get; init; }

        /// <inheritdoc />
        public DateTime UpdatedOn { get; init; }

        /// <inheritdoc />
        public bool IsDeleted { get; init; }

        /// <inheritdoc />
        public uint Timestamp { get; init; }
    }
}
