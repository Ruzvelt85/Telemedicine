namespace Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo
{
    /// <summary>
    /// Setting for connecting to video conferences
    /// </summary>
    public record VideoConferenceConnectionSettings
    {
        /// <summary>
        /// Minimal time (in seconds) before the beginning of the appointment when fetching the connection info is allowed
        /// </summary>
        public int TimeInSecondsBeforeAppointmentStartWhenGettingConnectionInfoAllowed { get; init; } = 60;
    }
}
