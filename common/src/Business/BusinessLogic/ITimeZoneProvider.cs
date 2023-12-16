using System;
using System.Threading.Tasks;

namespace Telemedicine.Common.Business.BusinessLogic
{
    public interface ITimeZoneProvider
    {
        Task<TimeSpan> GetPatientTimeOffset(Guid patientId);
    }
}
