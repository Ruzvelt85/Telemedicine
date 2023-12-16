using System;
using Telemedicine.Common.Infrastructure.Patterns.Command;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.Doctor
{
    public record UpdateDoctorCommand : ICommand<Guid>
    {
        public UpdateDoctorCommand(string firstName, string lastName)
        {
            LastName = lastName;
            FirstName = firstName;
        }

        public string FirstName { get; init; }

        public string LastName { get; init; }

        public bool IsDeleted { get; init; }
    }
}
