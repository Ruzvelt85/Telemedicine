using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Services;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Repositories;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Specifications;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Services
{
    /// <inheritdoc cref="IOverlappedAppointmentsService"/>
    public class OverlappedAppointmentsService : IOverlappedAppointmentsService
    {
        private readonly IAppointmentReadRepository _appointmentReadRepository;

        public OverlappedAppointmentsService(IAppointmentReadRepository appointmentReadRepository)
        {
            _appointmentReadRepository = appointmentReadRepository ?? throw new ArgumentNullException(nameof(appointmentReadRepository));
        }

        /// <inheritdoc />
        public Task<Appointment[]> GetOverlappedAppointments(DateTime startDate, TimeSpan duration, Guid[] attendeesIds, CancellationToken cancellationToken = default)
        {
            if (!attendeesIds.Any())
                return Task.FromResult(Array.Empty<Appointment>());

            var overlappedSpec = new AppointmentOverlappedSpecification(startDate, duration, attendeesIds);

            return _appointmentReadRepository
                .Find(overlappedSpec)
                .ToArrayAsync(cancellationToken);
        }
    }
}
