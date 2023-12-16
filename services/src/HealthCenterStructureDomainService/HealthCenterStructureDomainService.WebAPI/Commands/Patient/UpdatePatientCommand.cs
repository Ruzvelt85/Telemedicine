using System;
using Telemedicine.Common.Infrastructure.Patterns.Command;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.Patient
{
    public record UpdatePatientCommand : ICommand<Guid>
    {
        public UpdatePatientCommand(string lastName, string firstName, string? phoneNumber, DateTime? birthDate, Guid healthCenterId, Guid? primaryCareProviderId)
        {
            LastName = lastName;
            FirstName = firstName;
            PhoneNumber = phoneNumber;
            BirthDate = birthDate;
            HealthCenterId = healthCenterId;
            PrimaryCareProviderId = primaryCareProviderId;
        }

        public string LastName { get; init; }

        public string FirstName { get; init; }

        public string? PhoneNumber { get; init; }

        public DateTime? BirthDate { get; init; }

        public Guid HealthCenterId { get; init; }

        public Guid? PrimaryCareProviderId { get; init; }

        public bool IsDeleted { get; init; }
    }
}
