using System;
using System.Reflection;
using Autofac;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Settings;
using Telemedicine.Services.HealthMeasurementDomainService.DAL;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto;
using static Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities.LogConfiguringUtility;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI
{
    public class Startup : ServiceStartupBase
    {
        protected override Type EfCoreContextType => typeof(HealthMeasurementDomainServiceDbContext);

        /// <inheritdoc />
        protected override Assembly[] ValidationAssemblies => new[]
        {
            typeof(CreateMoodMeasurementRequestDtoValidator).Assembly
        };

        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            ConfigureTimeZone(services);
            services.Configure<SaturationMeasurementSettings>(Configuration.GetSection(nameof(SaturationMeasurementSettings)));
        }

        private void ConfigureTimeZone(IServiceCollection services)
        {
            StartupLogging.Logger.Information(StartExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureTimeZone));

            services.ConfigureSettings<TimeZoneSettings>(Configuration);

            StartupLogging.Logger.Information(EndExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureTimeZone));
        }

        protected override void RegisterServicesInIoC(ContainerBuilder builder)
        {
            base.RegisterServicesInIoC(builder);
            builder.RegisterType<SettingsTimeZoneProvider>().As<ITimeZoneProvider>().SingleInstance();
            builder.RegisterType<SaturationMeasurementSettingsBuilder>().As<ISaturationMeasurementSettingsBuilder>();
        }
    }
}
