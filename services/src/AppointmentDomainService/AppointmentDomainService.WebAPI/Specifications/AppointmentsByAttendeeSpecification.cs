using System;
using System.Linq;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Specifications
{
    /// <summary>
    /// Specification for searching appointments with specified attendee
    /// </summary>
    public record AppointmentsByAttendeeSpecification : Specification<Core.Entities.Appointment>
    {
        private readonly Guid _attendeeIdFilter;

        public AppointmentsByAttendeeSpecification(Guid attendeeIdFilter)
        {
            _attendeeIdFilter = attendeeIdFilter;
        }

        /// <inheritdoc />
        protected override Expression<Func<Core.Entities.Appointment, bool>> Predicate
            => appointment => appointment.Attendees.Any(x => x.UserId == _attendeeIdFilter);
    }
}
