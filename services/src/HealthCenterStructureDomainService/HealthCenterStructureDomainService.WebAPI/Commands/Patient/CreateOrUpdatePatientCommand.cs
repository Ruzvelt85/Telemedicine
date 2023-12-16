using System;
using Telemedicine.Common.Infrastructure.Patterns.Command;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.Patient
{
    public record CreateOrUpdatePatientCommand : ICommand<Guid?>
    {
        public CreateOrUpdatePatientCommand(string innerId, string lastName, string firstName, string? phoneNumber, DateTime? birthDate, string healthCenterInnerId, string? primaryCareProviderInnerId, bool? isActive)
        {
            InnerId = innerId;
            LastName = lastName;
            FirstName = firstName;
            PhoneNumber = phoneNumber;
            BirthDate = birthDate;
            HealthCenterInnerId = healthCenterInnerId;
            PrimaryCareProviderInnerId = primaryCareProviderInnerId;
            IsActive = isActive;
        }

        public string InnerId { get; init; }

        public string LastName { get; init; }

        public string FirstName { get; init; }

        public string? PhoneNumber { get; init; }

        public DateTime? BirthDate { get; init; }

        public string HealthCenterInnerId { get; init; }

        public string? PrimaryCareProviderInnerId { get; init; }

        public bool? IsActive { get; init; }
    }
}
