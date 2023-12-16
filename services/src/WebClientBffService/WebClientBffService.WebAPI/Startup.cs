using System.Reflection;
using Autofac;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientList;
using Telemedicine.Services.WebClientBffService.WebAPI.Services;

namespace Telemedicine.Services.WebClientBffService.WebAPI
{
    public class Startup : ServiceStartupBase
    {
        /// <inheritdoc />
        protected override Assembly[] ValidationAssemblies => new[]
        {
            typeof(PatientListRequestDto).Assembly,
            typeof(PagingRequestDtoValidator).Assembly
        };

        protected override bool IsUseAuthorization { get; set; } = true;

        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.ConfigureSettings<VideoConferenceConnectionSettings>(Configuration);
        }

        protected override void RegisterServicesInIoC(ContainerBuilder builder)
        {
            base.RegisterServicesInIoC(builder);
            builder.RegisterType<GetAppointmentConnectionInfoQueryHandler>().As<IQueryHandler<GetAppointmentConnectionInfoQuery, AppointmentConnectionInfoResponseDto>>();
            builder.RegisterType<HealthMeasurementAccessProvider>().As<IHealthMeasurementAccessProvider>();
        }
    }
}
