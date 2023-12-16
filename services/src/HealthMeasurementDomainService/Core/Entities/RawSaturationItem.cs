using System;

namespace Telemedicine.Services.HealthMeasurementDomainService.Core.Entities
{
    public class RawSaturationItem
    {
        public int Order { get; init; }

        /// <summary>
        /// Peripheral oxygen saturation
        /// </summary>
        public int SpO2 { get; init; }

        public int PulseRate { get; init; }

        /// <summary>
        /// Perfusion Index 
        /// </summary>
        public decimal Pi { get; init; }

        public DateTime ClientDate { get; init; }
    }
}
