using System;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Services
{
    public interface IHealthMeasurementAccessProvider
    {
        /// <summary>
        /// Gets the date of the last completed appointment for the patient. Compares the got date with the requested one.
        /// If requested dateRange.To is greater than completed, trimmed the requested dateRange.To to the last appointment end date.
        /// If requested dateRange.From greater than the last completed appointment end date returns null.
        /// Also, if there were no completed appointments, returns null.
        /// </summary>
        Task<Range<DateTime?>?> TrimDateFilterAccordingToLastAppointmentAsync(Guid patientId, Range<DateTime?> dateRange, CancellationToken cancellationToken = default);
    }
}
