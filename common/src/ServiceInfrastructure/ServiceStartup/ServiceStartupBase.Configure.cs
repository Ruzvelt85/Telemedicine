using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase;
using Telemedicine.Common.ServiceInfrastructure.ExceptionHandling.Service.Middleware;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities.Authorization;
using static Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities.LogConfiguringUtility;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup
{
    public abstract partial class ServiceStartupBase
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public virtual void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime, IWebHostEnvironment hostingEnvironments)
        {
            Log.Information(StartExecutingMessage, nameof(ServiceStartupBase), nameof(Configure));

            ApplicationLifetime = applicationLifetime;
            HostingEnvironments = hostingEnvironments;

            ConfigureAlways(app);

            if (IsProdCondition())
            {
                ConfigureIfProd(app);
            }
            else
            {
                ConfigureNotProd(app);
            }

            Log.Information(EndExecutingMessage, nameof(ServiceStartupBase), nameof(Configure));
        }

        /// <summary>
        /// Configure for all environments
        /// </summary>
        protected virtual IApplicationBuilder ConfigureAlways(IApplicationBuilder app)
        {
            Log.Information(StartExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureAlways));

            ConfigureExceptionHandling(app);

            app.UseRouting();

            ConfigureLog(app); // need to use after UseRouting to be able to use GetEndpoint() method

            HealthCheckConfigureUtility.Configure(app);

            app.UseCors();
            ConfigureAuthorization(app);

            app.UseLogUserIdHeaderMiddleware();
            app.UseHeaderPropagationWithConfiguration(Configuration);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks(HealthCheckConfigureUtility.DefaultHealthCheckUri);
                endpoints.MapControllers();
            });

            SwaggerConfigureUtility.Configure(app, ExecutingAssembly,
                Configuration.GetSettings<ServiceStartupSettings>() ?? ServiceStartupSettings.Default);

            Log.Information(EndExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureAlways));

            return app;
        }

        protected virtual IApplicationBuilder ConfigureAuthorization(IApplicationBuilder app)
        {
            Log.Information(StartExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureAuthorization));

            if (!IsUseAuthorization)
            {
                Log.Debug("Authorization is disabled");
                return app;
            }

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseIdentityInfoEnricher(IsUseAuthorization);

            Log.Information(EndExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureAuthorization));
            return app;
        }

        /// <summary>
        /// Configure for production environment
        /// </summary>
        protected virtual IApplicationBuilder ConfigureIfProd(IApplicationBuilder app) => app;

        /// <summary>
        /// Configure for non production environment
        /// </summary>
        protected virtual IApplicationBuilder ConfigureNotProd(IApplicationBuilder app)
        {
            Log.Information(StartExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureNotProd));

            app.UseDeveloperExceptionPage();

            Log.Information(EndExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureNotProd));

            return app;
        }

        /// <summary>
        /// Define environment
        /// </summary>
        protected virtual bool IsProdCondition() => HostingEnvironments.IsProduction();

        /// <summary>
        /// Query/Answer Logging Configuration and Logging when Service Stops
        /// </summary>
        protected virtual IApplicationBuilder ConfigureLog(IApplicationBuilder app)
        {
            Log.Information(StartExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureLog));

            app.ConfigureLogging(HostingEnvironments, ApplicationLifetime, StartupLogging.OnShutdown); //Registration of OWIN modules and Shutdown method
            app.UseCorrelationId();
            app.UseRequestResponseLogging();

            Log.Information(EndExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureLog));

            return app;
        }

        protected virtual IApplicationBuilder ConfigureExceptionHandling(IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            return app;
        }
    }
}
