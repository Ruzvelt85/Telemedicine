using System;

namespace Telemedicine.Services.MobileClientBffService.API.MobileClientQueryService.Dto
{
    public record AttendeeResponseDto
    {
        public AttendeeResponseDto(Guid id, string firstName, string lastName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
        }

        public Guid Id { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }

        public UserType UserType { get; init; }
    }
}
