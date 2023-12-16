using System;

namespace Telemedicine.Services.HealthMeasurementDomainService.Core.Contracts
{
    public interface IHasClientDate
    {
        DateTime ClientDate { get; }
    }
}
