using System;
using Telemedicine.Common.Infrastructure.Patterns.Command;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.Doctor
{
    public record CreateDoctorCommand : ICommand<Guid>
    {
        public CreateDoctorCommand(string innerId, string firstName, string lastName)
        {
            InnerId = innerId;
            LastName = lastName;
            FirstName = firstName;
        }

        public string InnerId { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }

        // TODO: JD-1282 - Remove this property from CreateCommand
        public bool IsDeleted { get; init; }
    }
}
