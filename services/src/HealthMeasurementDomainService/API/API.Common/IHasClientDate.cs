using System;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common
{
    public interface IHasClientDate
    {
        DateTime ClientDate { get; }
    }
}
