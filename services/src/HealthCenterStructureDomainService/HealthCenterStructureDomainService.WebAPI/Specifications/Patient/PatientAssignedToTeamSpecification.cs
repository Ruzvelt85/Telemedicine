using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications.Patient
{
    /// <summary>
    /// Specification to get only patient if he has InterdisciplinaryTeamId and this ID is included to list of team ids
    /// </summary>
    public record PatientAssignedToTeamSpecification : Specification<Core.Entities.Patient>
    {
        private readonly IEnumerable<Guid> _teamIds;

        public PatientAssignedToTeamSpecification(IEnumerable<Guid> teamIds)
        {
            _teamIds = teamIds;
        }

        /// <inheritdoc />
        protected override Expression<Func<Core.Entities.Patient, bool>> Predicate
            => patient => patient.InterdisciplinaryTeamId.HasValue && _teamIds.Contains(patient.InterdisciplinaryTeamId.Value);
    }
}
