// ReSharper disable UnusedMember.Global

using System.ComponentModel;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Settings
{
    public record JwtBearerClaimsSettings
    {
        [DefaultValue("sub")]
        public string UserId { get; init; } = "sub";
    }
}
