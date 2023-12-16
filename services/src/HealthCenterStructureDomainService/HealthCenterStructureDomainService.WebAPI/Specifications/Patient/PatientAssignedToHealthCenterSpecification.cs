using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications.Patient
{
    /// <summary>
    /// Specification to get only patient if he has HealthCenterId and this ID is included to list of HealthCenter ids
    /// </summary>
    public record PatientAssignedToHealthCenterSpecification : Specification<Core.Entities.Patient>
    {
        private readonly IEnumerable<Guid> _healthCenterIds;

        public PatientAssignedToHealthCenterSpecification(IEnumerable<Guid> healthCenterIds)
        {
            _healthCenterIds = healthCenterIds;
        }

        /// <inheritdoc />
        protected override Expression<Func<Core.Entities.Patient, bool>> Predicate
            => patient => _healthCenterIds.Contains(patient.HealthCenterId);
    }
}
