using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Enums;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities
{
    public class Doctor : User
    {
        [JsonConstructor]
        [Obsolete("Do not use this constructor for creating the entity; it's used in migrations and unit tests")]
        public Doctor(Guid id, string innerId, string firstName, string lastName, string phoneNumber, bool isAdmin, DateTime createdOn, DateTime updatedOn, bool isDeleted)
            : base(id, innerId, firstName, lastName, phoneNumber, UserType.Doctor, createdOn, updatedOn, isDeleted)
        {
            InterdisciplinaryTeams = new List<InterdisciplinaryTeam>();
            _healthCenters = new List<HealthCenter>();
            IsAdmin = isAdmin;
        }

        [Obsolete("Do not use this constructor for creating the entity; it's used by Automapper while creating an entity")]
        public Doctor(string innerId, string firstName, string lastName, bool isDeleted)
            : base(innerId, firstName, lastName, null, UserType.Doctor, isDeleted)
        {
            InterdisciplinaryTeams = new List<InterdisciplinaryTeam>();
            _healthCenters = new List<HealthCenter>();
        }

        // TODO: JD-1282 - Remove this constructor for Update Command, check InfrastructureTest after changes
        [Obsolete("Do not use this constructor for creating the entity; it's used by Automapper while updating an entity")]
        public Doctor(string firstName, string lastName, bool isDeleted)
            : base(firstName, lastName, null, UserType.Doctor, isDeleted)
        {
            InterdisciplinaryTeams = new List<InterdisciplinaryTeam>();
            _healthCenters = new List<HealthCenter>();
        }

        public bool IsAdmin { get; init; }

        public Doctor SetHealthCenters(IReadOnlyCollection<HealthCenter> healthCenters)
        {
            if (_healthCenters != null)
            {
                _healthCenters.Clear();
            }

            _healthCenters ??= new List<HealthCenter>();
            foreach (var healthCenter in healthCenters)
            {
                _healthCenters.Add(healthCenter);
            }

            return this;
        }

        #region NavigationProperties

        public ICollection<InterdisciplinaryTeam> InterdisciplinaryTeams { get; init; }

        public IReadOnlyCollection<HealthCenter> HealthCenters => new ReadOnlyCollection<HealthCenter>(_healthCenters?.ToArray() ?? Array.Empty<HealthCenter>());

        private ICollection<HealthCenter>? _healthCenters;

        #endregion
    }
}
