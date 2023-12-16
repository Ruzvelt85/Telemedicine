using System;
using System.Reflection;
using Autofac;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.VidyoClient;
using Telemedicine.Common.Infrastructure.VidyoClient.StartupSetupExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup;
using Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceCommandService.Dto;
using Telemedicine.Services.VideoConfIntegrService.DAL;
using Telemedicine.Services.VideoConfIntegrService.WebAPI.Services;

namespace Telemedicine.Services.VideoConfIntegrService.WebAPI
{
    public class Startup : ServiceStartupBase
    {
        protected override Type EfCoreContextType => typeof(VideoConferenceIntegrationServiceDbContext);

        /// <inheritdoc />
        protected override Assembly[] ValidationAssemblies => new[]
        {
            typeof(CreateConferenceRequestDtoValidator).Assembly,
            typeof(ConferenceSettingsValidator).Assembly
        };

        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void RegisterServicesInIoC(ContainerBuilder builder)
        {
            base.RegisterServicesInIoC(builder);

            builder.AddVidyoService();
        }

        protected override IHealthChecksBuilder ConfigureHealthCheck(IServiceCollection services)
        {
            IHealthChecksBuilder healthChecksBuilder = base.ConfigureHealthCheck(services);
            return healthChecksBuilder.AddVidyoService();
        }

        /// <exception cref="ConfigurationMissingException">Thrown when <see cref="VidyoServiceConnectionSettings"/> is missing.</exception>
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.ConfigureVidyoService(Configuration);

            services.ConfigureSettings<ConferenceSettings, ConferenceSettingsValidator>(Configuration);
            services.AddScoped<IConferenceFactory, ConferenceFactory>();
        }
    }
}
