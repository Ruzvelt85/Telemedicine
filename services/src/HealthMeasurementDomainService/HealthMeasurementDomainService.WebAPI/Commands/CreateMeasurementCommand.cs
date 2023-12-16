using System;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Commands
{
    public record CreateMeasurementCommand<TMeasure> : ICommand<Guid> where TMeasure : IMeasurement, new()
    {
        public Guid PatientId { get; init; }

        public TMeasure Measure { get; init; }

        public DateTime ClientDate { get; init; }

        public CreateMeasurementCommand(Guid patientId, DateTime clientDate, TMeasure measure)
        {
            PatientId = patientId;
            ClientDate = clientDate;
            Measure = measure;
        }
    }
}
