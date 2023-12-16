using System;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService.Dto
{
    public record UserInfoResponseDto
    {
        public UserInfoResponseDto(Guid id, string firstName, string lastName, UserType type)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Type = type;
        }

        public Guid Id { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }

        public UserType Type { get; init; }
    }
}
