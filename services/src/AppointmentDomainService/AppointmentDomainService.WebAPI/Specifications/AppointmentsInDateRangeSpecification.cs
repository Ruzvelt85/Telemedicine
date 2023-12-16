using System;
using System.Linq.Expressions;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Specifications
{
    /// <summary>
    /// Specification for searching appointments that will happen in specified date range
    /// </summary>
    public record AppointmentsInDateRangeSpecification : Specification<Core.Entities.Appointment>
    {
        private readonly Range<DateTime?> _dateRange;

        public AppointmentsInDateRangeSpecification(Range<DateTime?> dateRange)
        {
            _dateRange = dateRange;
        }

        /// <inheritdoc />
        protected override Expression<Func<Core.Entities.Appointment, bool>> Predicate
            => appointment => (!_dateRange.From.HasValue || _dateRange.From.Value <= appointment.StartDate)
                              && (!_dateRange.To.HasValue || appointment.StartDate < _dateRange.To.Value);
    }
}
