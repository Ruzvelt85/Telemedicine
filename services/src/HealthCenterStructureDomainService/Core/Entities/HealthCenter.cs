using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Telemedicine.Common.Infrastructure.EntityBase;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities
{
    public class HealthCenter : IEntity, IInnerIdSystem, ILogicallyDeletable, IAuditable, IOptimisticLock
    {
        [JsonConstructor]
        [Obsolete("Do not use this constructor for creating the entity; it's used in migrations and unit tests")]
        public HealthCenter(Guid id, string innerId, string name, string usaState, bool isDeleted, DateTime createdOn, DateTime updatedOn, uint timestamp)
        {
            Id = id;
            InnerId = innerId;
            Name = name;
            UsaState = usaState;
            IsDeleted = isDeleted;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
            Timestamp = timestamp;
            Doctors = new List<Doctor>();
        }

        [Obsolete("Do not use this constructor for creating the entity; it's used by Automapper while creating an entity")]
        public HealthCenter(string innerId, string name, string usaState, bool isDeleted)
        {
            InnerId = innerId;
            Name = name;
            UsaState = usaState;
            IsDeleted = isDeleted; // TODO: JD-1282 - Remove setting of this property while creating the entity
            Doctors = new List<Doctor>();
        }

        // TODO: Remove this constructor for Update Command, check InfrastructureTest after changes
        [Obsolete("Do not use this constructor for creating the entity; it's used by Automapper while updating an entity")]
#pragma warning disable 8618
        public HealthCenter(string name, string usaState, bool isDeleted)
#pragma warning restore 8618
        {
            Name = name;
            UsaState = usaState;
            IsDeleted = isDeleted;
            Doctors = new List<Doctor>();
        }

        /// <inheritdoc />
        public Guid Id { get; set; }

        public string InnerId { get; init; }

        public string Name { get; set; }

        public string UsaState { get; set; }

        /// <inheritdoc />
        public bool IsDeleted { get; init; }

        /// <inheritdoc />
        public DateTime CreatedOn { get; init; }

        /// <inheritdoc />
        public DateTime UpdatedOn { get; init; }

        /// <inheritdoc />
        public uint Timestamp { get; init; }

        #region NavigationProperties

        public ICollection<Doctor> Doctors { get; init; }

        #endregion
    }
}
