using System;
using Telemedicine.Common.Infrastructure.Patterns.Command;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.Doctor
{
    public record CreateOrUpdateDoctorCommand : ICommand<Guid?>
    {
        public CreateOrUpdateDoctorCommand(string innerId, string firstName, string lastName, string[] healthCenterInnerIds, bool isActive)
        {
            InnerId = innerId;
            LastName = lastName;
            FirstName = firstName;
            HealthCenterInnerIds = healthCenterInnerIds;
            IsActive = isActive;
        }

        public string InnerId { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }

        public string[] HealthCenterInnerIds { get; init; }

        public bool? IsActive { get; init; }
    }
}
