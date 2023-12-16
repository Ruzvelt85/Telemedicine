namespace Telemedicine.Common.Business.BusinessLogic
{
    public record TimeZoneSettings
    {
        private const string DefaultPatientTimeZoneId = "Central Standard Time";

        public string PatientTimeZoneId { get; init; } = DefaultPatientTimeZoneId;
    }
}
