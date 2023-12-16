using System;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService.Dto
{
    public record PatientInfoResponseDto : UserInfoDetailsResponseDto
    {
        /// <inheritdoc />
        public PatientInfoResponseDto(Guid id, string firstName, string lastName, UserType type, string innerId, string? phoneNumber, DateTime? birthDate)
            : base(id, firstName, lastName, type, innerId, phoneNumber)
        {
            BirthDate = birthDate;
        }

        public DateTime? BirthDate { get; init; }
    }
}
