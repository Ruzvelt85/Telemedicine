using JetBrains.Annotations;

namespace Telemedicine.Services.WebClientBffService.API.Common
{
    /// <summary>
    /// Type of appointment
    /// </summary>
    [PublicAPI]
    public enum AppointmentType
    {
        Default = 0,

        Urgent = 1,

        Annual = 2,

        SemiAnnual = 3,

        FollowUp = 4
    }
}
