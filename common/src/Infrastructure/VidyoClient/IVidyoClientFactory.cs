using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Unexpected;
using VidyoService;

namespace Telemedicine.Common.Infrastructure.VidyoClient
{
    /// <summary>
    /// Factory for <see cref="VidyoClient"/>
    /// </summary>
    public interface IVidyoClientFactory
    {
        /// <summary>
        /// Creates a configured object of <see cref="VidyoPortalUserServicePortTypeClient"/>
        /// </summary>
        /// <param name="vidyoSettings">Settings that are used for creating an object of <see cref="VidyoPortalUserServicePortTypeClient"/></param>
        /// <returns>Configured <see cref="VidyoPortalUserServicePortTypeClient"/></returns>
        /// <exception cref="ConfigurationValidationException">Thrown when the provided settings are not valid</exception>
        VidyoPortalUserServicePortTypeClient Create(VidyoServiceConnectionSettings vidyoSettings);
    }
}
