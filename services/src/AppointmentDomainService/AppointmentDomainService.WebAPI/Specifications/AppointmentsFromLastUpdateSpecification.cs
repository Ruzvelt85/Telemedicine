using System;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Specifications
{
    /// <summary>
    /// Specification for searching appointments that was updated later then last update
    /// </summary>
    public record AppointmentsFromLastUpdateSpecification : Specification<Appointment>
    {
        private readonly DateTime _lastUpdate;

        public AppointmentsFromLastUpdateSpecification(DateTime lastUpdate)
        {
            _lastUpdate = lastUpdate;
        }

        /// <inheritdoc />
        protected override Expression<Func<Appointment, bool>> Predicate =>
            appointment => appointment.UpdatedOn > _lastUpdate;
    }
}
