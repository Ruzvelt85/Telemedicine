using System;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Contracts;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Specifications
{
    public record PatientIdSpec : Specification<IHasPatientId>
    {
        private readonly Guid _patientIdFilter;

        public PatientIdSpec(Guid patientIdFilter)
        {
            _patientIdFilter = patientIdFilter;
        }

        /// <inheritdoc />
        protected override Expression<Func<IHasPatientId, bool>> Predicate => obj => obj.PatientId == _patientIdFilter;
    }
}
