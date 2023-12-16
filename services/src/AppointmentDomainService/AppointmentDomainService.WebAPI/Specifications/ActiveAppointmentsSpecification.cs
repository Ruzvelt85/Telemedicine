using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Specifications
{
    /// <summary>
    /// Specification for searching active appointments (in <see cref="AppointmentState.Opened"/> or <see cref="AppointmentState.Ongoing"/> state)
    /// </summary>
    public record ActiveAppointmentsSpecification : BinaryExpressionSpecification<Appointment>
    {
        public ActiveAppointmentsSpecification()
            : base(Expression.OrElse, new AppointmentsByStateSpecification(AppointmentState.Opened), new AppointmentsByStateSpecification(AppointmentState.Ongoing))
        { }
    }
}
