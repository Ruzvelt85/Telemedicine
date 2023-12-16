using System;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;

namespace Telemedicine.Services.AppointmentDomainService.Core.Services
{
    /// <summary>
    /// Service that encapsulates logic regarding appointments overlapping
    /// </summary>
    public interface IOverlappedAppointmentsService
    {
        /// <summary>
        /// Returns a collection of overlapped appointments for given attendees
        /// </summary>
        /// <param name="startDate">Appointment start date</param>
        /// <param name="duration">Appointment duration</param>
        /// <param name="attendeesIds">Attendees Ids for whose overlapped appointments will be returned</param>
        /// <param name="cancellationToken"></param>
        Task<Appointment[]> GetOverlappedAppointments(DateTime startDate, TimeSpan duration, Guid[] attendeesIds, CancellationToken cancellationToken = default);
    }
}
