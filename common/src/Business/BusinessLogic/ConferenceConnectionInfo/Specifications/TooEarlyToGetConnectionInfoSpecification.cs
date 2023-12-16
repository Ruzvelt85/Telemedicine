using System;
using System.Linq.Expressions;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Dto;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;

namespace Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Specifications
{
    /// <summary>
    /// Specification checks if it's too early to get connection info
    /// </summary>
    public record TooEarlyToGetConnectionInfoSpecification : Specification<AppointmentInfoDto>
    {
        private readonly int _secondsBeforeAppointmentStartDate;

        public TooEarlyToGetConnectionInfoSpecification(int secondsBeforeAppointmentStartDate)
        {
            _secondsBeforeAppointmentStartDate = secondsBeforeAppointmentStartDate;
        }

        /// <inheritdoc />
        protected override Expression<Func<AppointmentInfoDto, bool>> Predicate =>
            appointment => DateTime.UtcNow < appointment.StartDate.AddSeconds(-_secondsBeforeAppointmentStartDate);
    }
}
