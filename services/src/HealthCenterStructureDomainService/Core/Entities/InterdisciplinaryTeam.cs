using System;
using System.Collections.Generic;
using Telemedicine.Common.Infrastructure.EntityBase;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities
{
    public class InterdisciplinaryTeam : IEntity, IInnerIdSystem, IAuditable, ILogicallyDeletable, IOptimisticLock
    {
        [Obsolete("Do not use this constructor for creating the entity")]
        public InterdisciplinaryTeam(Guid id, string innerId, string name, Guid healthCenterId, DateTime createdOn, DateTime updatedOn, bool isDeleted, uint timestamp)
        {
            Id = id;
            InnerId = innerId;
            Name = name;
            HealthCenterId = healthCenterId;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
            IsDeleted = isDeleted;
            Timestamp = timestamp;
            Doctors = new List<Doctor>();
        }

        public Guid Id { get; set; }

        public string InnerId { get; init; }

        public string Name { get; init; }

        public Guid HealthCenterId { get; init; }

        /// <inheritdoc />
        public DateTime CreatedOn { get; init; }

        /// <inheritdoc />
        public DateTime UpdatedOn { get; init; }

        /// <inheritdoc />
        public bool IsDeleted { get; init; }

        /// <inheritdoc />
        public uint Timestamp { get; init; }

        #region NavigationProperties

        public ICollection<Doctor> Doctors { get; init; }

        #endregion
    }
}
