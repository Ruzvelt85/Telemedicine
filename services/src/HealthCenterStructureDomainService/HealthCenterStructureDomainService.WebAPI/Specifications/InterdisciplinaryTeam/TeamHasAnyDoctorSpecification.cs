using System;
using System.Linq;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications.InterdisciplinaryTeam
{
    /// <summary>
    /// Specification to get only InterdisciplinaryTeam if it contains doctor
    /// </summary>
    public record TeamHasAnyDoctorSpecification : Specification<Core.Entities.InterdisciplinaryTeam>
    {
        private readonly Guid _doctorId;

        public TeamHasAnyDoctorSpecification(Guid doctorId)
        {
            _doctorId = doctorId;
        }

        /// <inheritdoc />
        protected override Expression<Func<Core.Entities.InterdisciplinaryTeam, bool>> Predicate =>
            idt => idt.Doctors.Any(me => me.Id == _doctorId);
    }
}
