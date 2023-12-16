using System;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;

namespace Telemedicine.Services.HealthCenterStructureDomainService.DAL.CrossEntities
{
    public class InterdisciplinaryTeamDoctor
    {
        public Guid InterdisciplinaryTeamId { get; init; }

        public Guid DoctorId { get; init; }

        public InterdisciplinaryTeam InterdisciplinaryTeam { get; init; } = null!;

        public Doctor Doctor { get; init; } = null!;
    }
}
