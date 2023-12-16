using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Telemedicine.Services.AppointmentDomainService.Core.Entities
{
    /// <summary>
    /// Specification for searching appointments that will happen in specified state
    /// </summary>
    public record AppointmentsByStateSpecification : Specification<Appointment>
    {
        private readonly AppointmentState _state;
        public AppointmentsByStateSpecification(AppointmentState state)
        {
            _state = state;
        }

        /// <inheritdoc />
        protected override Expression<Func<Appointment, bool>> Predicate =>
            _state switch
            {
                AppointmentState.Missed => appointment => appointment.Status == AppointmentStatus.Missed,
                AppointmentState.Cancelled => appointment => appointment.Status == AppointmentStatus.Cancelled,
                AppointmentState.Done => appointment => appointment.Status == AppointmentStatus.Opened
                                                        && appointment.StartDate + appointment.Duration < DateTime.UtcNow,
                AppointmentState.Ongoing => appointment => appointment.Status == AppointmentStatus.Opened
                                                           && appointment.StartDate <= DateTime.UtcNow
                                                           && appointment.StartDate + appointment.Duration >= DateTime.UtcNow,
                AppointmentState.Opened => appointment => appointment.Status == AppointmentStatus.Opened
                                                        && appointment.StartDate > DateTime.UtcNow,
                _ => appointment => false
            };

        public static List<KeyValuePair<Func<Appointment, bool>, AppointmentState>> StatePredicates { get; } = GetStatePredicates();

        private static List<KeyValuePair<Func<Appointment, bool>, AppointmentState>> GetStatePredicates()
        {
            var list = new List<KeyValuePair<Func<Appointment, bool>, AppointmentState>>();
            var states = (AppointmentState[])Enum.GetValues(typeof(AppointmentState));
            foreach (var state in states)
            {
                var specification = new AppointmentsByStateSpecification(state);
                list.Add(KeyValuePair.Create(specification.Predicate.Compile(), state));
            }

            return list;
        }
    }
}
