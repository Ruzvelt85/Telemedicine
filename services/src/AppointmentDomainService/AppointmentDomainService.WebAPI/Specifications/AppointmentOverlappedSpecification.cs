using System;
using System.Linq;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Specifications
{
    /// <summary>
    /// Specification for searching opened appointments that has overlaps with interval (from <see cref="_startDate"/> to <see cref="_endDate"/>) and attendees <see cref="_attendeeIds"/>
    /// </summary>
    public record AppointmentOverlappedSpecification : Specification<Appointment>
    {
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;
        private readonly Guid[] _attendeeIds;

        public AppointmentOverlappedSpecification(DateTime startDate, TimeSpan duration, params Guid[] attendeeIds)
        {
            _startDate = startDate;
            _attendeeIds = attendeeIds;
            _endDate = startDate + duration;
        }

        /// <inheritdoc />
        protected override Expression<Func<Appointment, bool>> Predicate =>
            appointment => appointment.Status == AppointmentStatus.Opened
                           && appointment.StartDate < _endDate
                           && _startDate < appointment.StartDate + appointment.Duration
                           && appointment.Attendees.Any(x => _attendeeIds.Contains(x.UserId));
    }
}
