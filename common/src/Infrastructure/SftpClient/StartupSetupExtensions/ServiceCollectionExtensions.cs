using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;

namespace Telemedicine.Common.Infrastructure.SftpClient.StartupSetupExtensions
{
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configure SFTP client <see cref="ISftpClient"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection ConfigureSftpClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureSettings<SftpClientSettings, SftpClientSettingsValidator>(configuration);
            var healthCheckBuilder = services.AddHealthChecks();
            healthCheckBuilder.AddSftp();

            return services;
        }
    }
}
