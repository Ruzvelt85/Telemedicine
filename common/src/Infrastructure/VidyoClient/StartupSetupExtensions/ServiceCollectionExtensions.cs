using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions;

namespace Telemedicine.Common.Infrastructure.VidyoClient.StartupSetupExtensions
{
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configure Vidyo client <see cref="IVidyoClient"/>.
        /// </summary>
        /// <exception cref="ConfigurationMissingException">Thrown when <see cref="VidyoServiceConnectionSettings"/> is missing.</exception>
        public static IServiceCollection ConfigureVidyoService(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureSettings<VidyoServiceConnectionSettings, VidyoServiceConnectionSettingsValidator>(configuration);
            return services;
        }
    }
}
