using System;
using System.Linq;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications.Doctor
{
    /// <summary>
    /// Specification to get doctor with specified health centers
    /// </summary>
    public record DoctorByHealthCenterSpecification : Specification<Core.Entities.Doctor>
    {
        private readonly Guid[] _healthCenterIds;

        public DoctorByHealthCenterSpecification(Guid[] healthCenterIds)
        {
            _healthCenterIds = healthCenterIds;
        }

        /// <inheritdoc />
        protected override Expression<Func<Core.Entities.Doctor, bool>> Predicate =>
            doctor => doctor.HealthCenters.Any(healthCenter => _healthCenterIds.Contains(healthCenter.Id));
    }
}
