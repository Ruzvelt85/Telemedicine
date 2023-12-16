using System;

namespace Telemedicine.Services.HealthMeasurementDomainService.Core.Contracts
{
    public interface IHasPatientId
    {
        Guid PatientId { get; }
    }
}
