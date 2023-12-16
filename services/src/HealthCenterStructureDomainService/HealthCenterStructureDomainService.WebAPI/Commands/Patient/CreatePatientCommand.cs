using System;
using Telemedicine.Common.Infrastructure.Patterns.Command;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.Patient
{
    public record CreatePatientCommand : ICommand<Guid>
    {
        public CreatePatientCommand(string innerId, string lastName, string firstName, string? phoneNumber, DateTime? birthDate, Guid healthCenterId, Guid? primaryCareProviderId)
        {
            InnerId = innerId;
            LastName = lastName;
            FirstName = firstName;
            PhoneNumber = phoneNumber;
            BirthDate = birthDate;
            HealthCenterId = healthCenterId;
            PrimaryCareProviderId = primaryCareProviderId;
        }

        public string InnerId { get; init; }

        public string LastName { get; init; }

        public string FirstName { get; init; }

        public string? PhoneNumber { get; init; }

        public DateTime? BirthDate { get; init; }

        public Guid HealthCenterId { get; init; }

        public Guid? PrimaryCareProviderId { get; init; }

        // TODO: JD-1282 - Remove this property from CreateCommand
        public bool IsDeleted { get; init; }
    }
}
