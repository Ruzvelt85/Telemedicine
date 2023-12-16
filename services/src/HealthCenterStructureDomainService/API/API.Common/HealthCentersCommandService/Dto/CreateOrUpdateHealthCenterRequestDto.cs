namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.HealthCentersCommandService.Dto
{
    public record CreateOrUpdateHealthCenterRequestDto
    {
        public string InnerId { get; init; } = string.Empty;

        public string Name { get; init; } = string.Empty;

        public string UsaState { get; init; } = string.Empty;

        public bool? IsActive { get; init; }
    }
}
