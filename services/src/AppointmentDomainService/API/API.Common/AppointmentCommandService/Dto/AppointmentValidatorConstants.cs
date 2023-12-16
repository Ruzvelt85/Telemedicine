using System;

namespace Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto
{
    public static class AppointmentValidatorConstants
    {
        public static readonly TimeSpan MinDuration = TimeSpan.FromMinutes(15);

        public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(3);

        public const int ReasonMaxLength = 100;
    }
}
