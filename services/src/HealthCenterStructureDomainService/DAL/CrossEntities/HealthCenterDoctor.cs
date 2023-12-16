using System;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;

namespace Telemedicine.Services.HealthCenterStructureDomainService.DAL.CrossEntities
{
    public class HealthCenterDoctor
    {
        public Guid HealthCenterId { get; init; }

        public Guid DoctorId { get; init; }

        public HealthCenter HealthCenter { get; init; } = null!;

        public Doctor Doctor { get; init; } = null!;
    }
}
