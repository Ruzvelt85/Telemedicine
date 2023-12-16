using System;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService.Dto
{
    public record DoctorInfoResponseDto : UserInfoDetailsResponseDto
    {
        /// <inheritdoc />
        public DoctorInfoResponseDto(Guid id, string firstName, string lastName, UserType type, string innerId, string? phoneNumber)
            : base(id, firstName, lastName, type, innerId, phoneNumber)
        {
        }
    }
}
