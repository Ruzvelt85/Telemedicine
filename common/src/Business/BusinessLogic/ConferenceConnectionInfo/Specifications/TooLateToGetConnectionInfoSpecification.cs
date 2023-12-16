using System;
using System.Linq.Expressions;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Dto;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;

namespace Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Specifications
{
    /// <summary>
    /// Specification checks if it's too late to get connection info
    /// </summary>
    public record TooLateToGetConnectionInfoSpecification : Specification<AppointmentInfoDto>
    {
        /// <inheritdoc />
        protected override Expression<Func<AppointmentInfoDto, bool>> Predicate =>
            appointment => DateTime.UtcNow > appointment.StartDate.Add(appointment.Duration);
    }
}
