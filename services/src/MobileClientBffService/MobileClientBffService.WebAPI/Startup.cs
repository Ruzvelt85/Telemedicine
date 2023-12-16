using Autofac;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Dto;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup;
using Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto;
using Telemedicine.Services.MobileClientBffService.API.Settings;

namespace Telemedicine.Services.MobileClientBffService.WebAPI
{
    public class Startup : ServiceStartupBase
    {
        /// <inheritdoc />
        protected override Assembly[] ValidationAssemblies => new[]
        {
            typeof(CreateMoodMeasurementRequestDtoValidator).Assembly
        };

        protected override bool IsUseAuthorization { get; set; } = true;

        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.ConfigureSettings<VideoConferenceConnectionSettings>(Configuration);
            services.Configure<SaturationMeasurementSettings>(Configuration.GetSection(nameof(SaturationMeasurementSettings)));
        }

        protected override void RegisterServicesInIoC(ContainerBuilder builder)
        {
            base.RegisterServicesInIoC(builder);
            builder.RegisterType<SaturationMeasurementSettingsBuilder>().As<ISaturationMeasurementSettingsBuilder>();
            builder.RegisterType<GetAppointmentConnectionInfoQueryHandler>().As<IQueryHandler<GetAppointmentConnectionInfoQuery, AppointmentConnectionInfoResponseDto>>();
        }
    }
}
