using System;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common;
using JsonKnownTypes;
using Newtonsoft.Json;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService.Dto
{
    [JsonConverter(typeof(JsonKnownTypesConverter<UserInfoDetailsResponseDto>))]
    public record UserInfoDetailsResponseDto : UserInfoResponseDto
    {
        public UserInfoDetailsResponseDto(Guid id, string firstName, string lastName, UserType type, string innerId, string? phoneNumber)
            : base(id, firstName, lastName, type)
        {
            InnerId = innerId;
            PhoneNumber = phoneNumber;
        }

        public string InnerId { get; init; }

        public string? PhoneNumber { get; init; }
    }
}
