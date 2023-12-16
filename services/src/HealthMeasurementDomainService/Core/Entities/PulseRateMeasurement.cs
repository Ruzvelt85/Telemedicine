using System;
using Newtonsoft.Json;
using Telemedicine.Common.Infrastructure.EntityBase;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Contracts;

namespace Telemedicine.Services.HealthMeasurementDomainService.Core.Entities
{
    public class PulseRateMeasurement : IEntity, IAuditable, IHasPatientId, IHasClientDate
    {
        // Json attribute for creating instance while seeding
        [JsonConstructor]
        [Obsolete("Do not use this constructor for creating the entity")]
        public PulseRateMeasurement(Guid id, Guid patientId, int pulseRate, DateTime clientDate,
            DateTime createdOn, DateTime updatedOn)
        {
            Id = id;
            PatientId = patientId;
            PulseRate = pulseRate;
            ClientDate = clientDate;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
        }

        public PulseRateMeasurement(Guid patientId, int pulseRate, DateTime clientDate)
        {
            PatientId = patientId;
            PulseRate = pulseRate;
            ClientDate = clientDate;
        }

        public Guid Id { get; init; }

        public Guid PatientId { get; init; }

        public int PulseRate { get; init; }

        public DateTime ClientDate { get; init; }

        /// <inheritdoc />
        public DateTime CreatedOn { get; init; }

        /// <inheritdoc />
        public DateTime UpdatedOn { get; init; }
    }
}
