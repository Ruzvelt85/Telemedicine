using System;

namespace Telemedicine.Services.WebClientBffService.API.Common
{
    [Flags]
    public enum MeasurementType
    {
        All = BloodPressure | Saturation | PulseRate | Mood,

        BloodPressure = 1,

        Saturation = 2,

        PulseRate = 4,

        Mood = 8
    }
}
