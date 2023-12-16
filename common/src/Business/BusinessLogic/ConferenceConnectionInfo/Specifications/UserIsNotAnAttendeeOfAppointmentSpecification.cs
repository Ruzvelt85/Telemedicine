using System;
using System.Linq;
using System.Linq.Expressions;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Dto;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;

namespace Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Specifications
{
    /// <summary>
    /// Specification checks if the specified user is not an attendee of the appointment
    /// </summary>
    public record UserIsNotAnAttendeeOfAppointmentSpecification : Specification<AppointmentInfoDto>
    {
        private readonly Guid _currentUserId;

        public UserIsNotAnAttendeeOfAppointmentSpecification(Guid currentUserId)
        {
            _currentUserId = currentUserId;
        }

        /// <inheritdoc />
        protected override Expression<Func<AppointmentInfoDto, bool>> Predicate =>
            appointment => !appointment.Attendees.Contains(_currentUserId);
    }
}
