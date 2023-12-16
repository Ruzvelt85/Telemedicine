using System;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Specifications
{
    /// <summary>
    /// Specification for searching appointments that will happen after the specified date
    /// </summary>
    public record NextAppointmentsSpecification : Specification<Core.Entities.Appointment>
    {
        private readonly DateTime _dateTimeFilter;

        public NextAppointmentsSpecification(DateTime dateTimeFilter)
        {
            _dateTimeFilter = dateTimeFilter;
        }

        /// <inheritdoc />
        protected override Expression<Func<Core.Entities.Appointment, bool>> Predicate
            => appointment => appointment.StartDate > _dateTimeFilter;
    }
}
