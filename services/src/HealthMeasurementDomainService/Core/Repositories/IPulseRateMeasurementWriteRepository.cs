﻿using Telemedicine.Common.Infrastructure.Patterns.Data;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;

namespace Telemedicine.Services.HealthMeasurementDomainService.Core.Repositories
{
    public interface IPulseRateMeasurementWriteRepository : IWriteRepository<PulseRateMeasurement>
    {
    }
}