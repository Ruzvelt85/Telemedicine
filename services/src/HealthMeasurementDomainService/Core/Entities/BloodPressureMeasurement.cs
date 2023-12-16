using System;
using Newtonsoft.Json;
using Telemedicine.Common.Infrastructure.EntityBase;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Contracts;

namespace Telemedicine.Services.HealthMeasurementDomainService.Core.Entities
{
    public class BloodPressureMeasurement : IEntity, IAuditable, IHasPatientId, IHasClientDate
    {
        // Json attribute for creating instance while seeding
        [JsonConstructor]
        [Obsolete("Do not use this constructor for creating the entity")]
        public BloodPressureMeasurement(Guid id, Guid patientId, int systolic, int diastolic,
            int pulseRate, DateTime clientDate, DateTime createdOn, DateTime updatedOn)
        {
            Id = id;
            PatientId = patientId;
            Systolic = systolic;
            Diastolic = diastolic;
            PulseRate = pulseRate;
            ClientDate = clientDate;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
        }

        public BloodPressureMeasurement(Guid patientId, DateTime clientDate, int systolic, int diastolic, int pulseRate)
        {
            PatientId = patientId;
            ClientDate = clientDate;
            Systolic = systolic;
            Diastolic = diastolic;
            PulseRate = pulseRate;
        }

        public Guid Id { get; init; }

        public Guid PatientId { get; init; }

        public int Systolic { get; init; }

        public int Diastolic { get; init; }

        public int PulseRate { get; init; }

        public DateTime ClientDate { get; init; }

        /// <inheritdoc />
        public DateTime CreatedOn { get; init; }

        /// <inheritdoc />
        public DateTime UpdatedOn { get; init; }
    }
}
