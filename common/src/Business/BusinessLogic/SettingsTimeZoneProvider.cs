using System;
using System.Threading.Tasks;
using TimeZoneConverter;
using Microsoft.Extensions.Options;

namespace Telemedicine.Common.Business.BusinessLogic
{
    public class SettingsTimeZoneProvider : ITimeZoneProvider
    {
        private readonly TimeZoneSettings _settings;

        public SettingsTimeZoneProvider(IOptions<TimeZoneSettings> settings)
        {
            _settings = settings.Value;
        }

        public Task<TimeSpan> GetPatientTimeOffset(Guid patientId)
        {
            var timeZoneId = _settings.PatientTimeZoneId;
            var timeZone = TZConvert.GetTimeZoneInfo(timeZoneId);
            return Task.FromResult(timeZone.BaseUtcOffset);
        }
    }
}
