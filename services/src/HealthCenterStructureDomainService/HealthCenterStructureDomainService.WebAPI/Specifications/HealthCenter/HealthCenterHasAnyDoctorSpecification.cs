using System;
using System.Linq;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications.HealthCenter
{
    /// <summary>
    /// Specification to get only HealthCenter if it contains doctor
    /// </summary>
    public record HealthCenterHasAnyDoctorSpecification : Specification<Core.Entities.HealthCenter>
    {
        private readonly Guid _doctorId;

        public HealthCenterHasAnyDoctorSpecification(Guid doctorId)
        {
            _doctorId = doctorId;
        }

        /// <inheritdoc />
        protected override Expression<Func<Core.Entities.HealthCenter, bool>> Predicate =>
            healthCenter => healthCenter.Doctors.Any(me => me.Id == _doctorId);
    }
}
