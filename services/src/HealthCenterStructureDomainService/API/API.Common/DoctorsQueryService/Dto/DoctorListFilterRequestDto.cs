using System;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto
{
    public record DoctorListFilterRequestDto
    {
        // We use here array of Guid (not a ReadOnlyCollection) due to the problem with binding values from query string from GET request to collection
        // Some details are here - https://github.com/aspnet/Mvc/issues/7712
        public Guid[]? HealthCenterIds { get; init; }

        public string? Name { get; init; }
    }
}
