namespace Telemedicine.Services.AppointmentDomainService.API.Common.Common
{
    /// <summary>
    /// Type of appointment
    /// </summary>
    public enum AppointmentType
    {
        Default = 0,

        Urgent = 1,

        Annual = 2,

        SemiAnnual = 3,

        FollowUp = 4
    }
}
