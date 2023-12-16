using System;
using Telemedicine.Common.Infrastructure.EntityBase;
using Newtonsoft.Json;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Contracts;

namespace Telemedicine.Services.HealthMeasurementDomainService.Core.Entities
{
    public class SaturationMeasurement : IEntity, IAuditable, IHasPatientId, IHasClientDate
    {
        // Json attribute for creating instance while seeding
        [JsonConstructor]
        [Obsolete("Do not use this constructor for creating the entity")]
        public SaturationMeasurement(Guid id, Guid patientId, int spO2, int pulseRate, decimal pi,
            DateTime clientDate, DateTime createdOn, DateTime updatedOn)
        {
            Id = id;
            PatientId = patientId;
            SpO2 = spO2;
            PulseRate = pulseRate;
            Pi = pi;
            ClientDate = clientDate;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
        }

        public SaturationMeasurement(Guid patientId, int spO2, int pulseRate, decimal pi,
            RawSaturationData? rawSaturationData, DateTime clientDate)
        {
            PatientId = patientId;
            ClientDate = clientDate;
            SpO2 = spO2;
            PulseRate = pulseRate;
            Pi = pi;
            RawSaturationData = rawSaturationData;
            ClientDate = clientDate;
        }

        public Guid Id { get; init; }

        public Guid PatientId { get; init; }

        /// <summary>
        /// Peripheral oxygen saturation
        /// </summary>
        public int SpO2 { get; init; }

        public int PulseRate { get; init; }

        /// <summary>
        /// Perfusion Index 
        /// </summary>
        public decimal Pi { get; init; }

        public RawSaturationData? RawSaturationData { get; init; }

        public DateTime ClientDate { get; init; }

        /// <inheritdoc />
        public DateTime CreatedOn { get; init; }

        /// <inheritdoc />
        public DateTime UpdatedOn { get; init; }
    }
}
