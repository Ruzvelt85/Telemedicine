using System;
using Newtonsoft.Json;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Enums;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities
{
    public class Patient : User
    {
        [JsonConstructor]
        [Obsolete("Do not use this constructor for creating the entity")]
        public Patient(Guid id, string innerId, string firstName, string lastName, string? phoneNumber, DateTime? birthDate,
            Guid? interdisciplinaryTeamId, Guid healthCenterId, Guid? primaryCareProviderId, DateTime createdOn, DateTime updatedOn, bool isDeleted)
            : base(id, innerId, firstName, lastName, phoneNumber, UserType.Patient, createdOn, updatedOn, isDeleted)
        {
            BirthDate = birthDate;
            InterdisciplinaryTeamId = interdisciplinaryTeamId;
            HealthCenterId = healthCenterId;
            PrimaryCareProviderId = primaryCareProviderId;
        }

        [Obsolete("Do not use this constructor for creating the entity; it's used by Automapper while creating an entity")]
        public Patient(string innerId, string firstName, string lastName, string? phoneNumber, DateTime? birthDate, Guid healthCenterId, Guid? primaryCareProviderId, bool isDeleted)
            : base(innerId, firstName, lastName, phoneNumber, UserType.Patient, isDeleted)
        {
            BirthDate = birthDate;
            HealthCenterId = healthCenterId;
            PrimaryCareProviderId = primaryCareProviderId;
        }

        // TODO: Remove this constructor for Update Command, check InfrastructureTest after changes
        [Obsolete("Do not use this constructor for creating the entity; it's used by Automapper while updating an entity")]
        public Patient(string firstName, string lastName, string? phoneNumber, DateTime? birthDate, Guid healthCenterId, Guid? primaryCareProviderId, bool isDeleted)
            : base(firstName, lastName, phoneNumber, UserType.Patient, isDeleted)
        {
            BirthDate = birthDate;
            HealthCenterId = healthCenterId;
            PrimaryCareProviderId = primaryCareProviderId;
        }

        public DateTime? BirthDate { get; init; }

        public Guid? InterdisciplinaryTeamId { get; init; }

        public Guid HealthCenterId { get; init; }

        public Guid? PrimaryCareProviderId { get; init; }

        #region NavigationProperties

        public HealthCenter? HealthCenter { get; init; }

        public InterdisciplinaryTeam? InterdisciplinaryTeam { get; init; }

        public Doctor? PrimaryCareProvider { get; init; }

        #endregion
    }
}
