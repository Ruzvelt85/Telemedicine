using System;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientList
{
    public record PatientResponseDto
    {
        public PatientResponseDto(Guid id, string innerId, string firstName, string lastName, string? phoneNumber)
        {
            Id = id;
            InnerId = innerId;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
        }

        public Guid Id { get; set; }

        public string InnerId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
