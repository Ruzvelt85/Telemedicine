using System;
using System.Reflection;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup;
using Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;
using Telemedicine.Services.AppointmentDomainService.Core.Services;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI
{
    public class Startup : ServiceStartupBase
    {
        protected override Type EfCoreContextType => typeof(AppointmentDomainServiceDbContext);

        protected override bool IsUseEventBus => true;

        /// <inheritdoc />
        protected override Assembly[] ValidationAssemblies => new[]
        {
            typeof(PagingRequestDtoValidator).Assembly,
            typeof(AppointmentListRequestDtoValidator).Assembly
        };

        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void ConfigureServicesInternal(IServiceCollection services)
        {
            base.ConfigureServicesInternal(services);

            services.AddScoped<IOverlappedAppointmentsService, OverlappedAppointmentsService>();
        }
    }
}
