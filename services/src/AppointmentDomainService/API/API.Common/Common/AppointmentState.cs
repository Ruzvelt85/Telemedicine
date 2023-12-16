namespace Telemedicine.Services.AppointmentDomainService.API.Common.Common
{
    /// <summary>
    /// State of appointment
    /// </summary>
    public enum AppointmentState
    {
        Default = 0,

        Opened = 1,

        Ongoing = 2,

        Cancelled = 3,

        Missed = 4,

        Done = 5,

        All = 1024
    }
}
