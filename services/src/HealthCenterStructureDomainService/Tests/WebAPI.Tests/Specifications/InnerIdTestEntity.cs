using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.Specifications
{
    public class InnerIdTestEntity : IInnerIdSystem
    {
        public string InnerId { get; init; } = string.Empty;
    }
}
