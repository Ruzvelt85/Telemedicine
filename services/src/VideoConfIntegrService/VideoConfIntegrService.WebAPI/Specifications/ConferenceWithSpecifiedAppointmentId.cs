using System;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Services.VideoConfIntegrService.Core.Entities;

namespace Telemedicine.Services.VideoConfIntegrService.WebAPI.Specifications
{
    /// <summary>
    /// Specification for seeking the conference with specified AppointmentId
    /// </summary>
    public record ConferenceWithSpecifiedAppointmentId : Specification<Conference>
    {
        private readonly Guid _appointmentId;

        public ConferenceWithSpecifiedAppointmentId(Guid appointmentId)
        {
            _appointmentId = appointmentId;
        }

        /// <inheritdoc />
        protected override Expression<Func<Conference, bool>> Predicate =>
            conference => conference.AppointmentId == _appointmentId;
    }
}
