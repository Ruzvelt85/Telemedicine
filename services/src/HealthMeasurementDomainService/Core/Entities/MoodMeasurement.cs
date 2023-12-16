using System;
using Newtonsoft.Json;
using Telemedicine.Common.Infrastructure.EntityBase;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Contracts;

namespace Telemedicine.Services.HealthMeasurementDomainService.Core.Entities
{
    public class MoodMeasurement : IEntity, IAuditable, IHasPatientId, IHasClientDate
    {
        // Json attribute for creating instance while seeding
        [JsonConstructor]
        [Obsolete("Do not use this constructor for creating the entity")]
        public MoodMeasurement(Guid id, Guid patientId, MoodMeasureType measure, DateTime clientDate,
            DateTime createdOn, DateTime updatedOn)
        {
            Id = id;
            PatientId = patientId;
            Measure = measure;
            ClientDate = clientDate;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
        }

        public MoodMeasurement(Guid patientId, MoodMeasureType measure, DateTime clientDate)
        {
            PatientId = patientId;
            Measure = measure;
            ClientDate = clientDate;
        }

        public Guid Id { get; init; }

        public Guid PatientId { get; init; }

        public MoodMeasureType Measure { get; init; }

        public DateTime ClientDate { get; init; }

        /// <inheritdoc />
        public DateTime CreatedOn { get; init; }

        /// <inheritdoc />
        public DateTime UpdatedOn { get; init; }
    }
}
