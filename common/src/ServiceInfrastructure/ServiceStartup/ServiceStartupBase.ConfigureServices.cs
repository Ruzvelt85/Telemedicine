using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using AutoMapper.Internal;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection;
using Serilog;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.DomainEventInfrastructure;
using Telemedicine.Common.Infrastructure.EventBus.SqsSender;
using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase;
using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.Middleware;
using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.Settings;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.ServiceInfrastructure.ExceptionHandling.Service.Settings;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Settings;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities.Authorization;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Controllers;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Filters;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Extensions;
using static Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities.LogConfiguringUtility;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup
{
    public abstract partial class ServiceStartupBase
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public virtual void ConfigureServices(IServiceCollection services) => ConfigureServicesInternal(services);

        /// <summary>
        /// ASP.NET Core method to configure IoC/DI container
        /// </summary>
        public void ConfigureContainer(ContainerBuilder builder) => builder.RegisterModule(new AutofacModule(this));

        /// <summary>
        /// This method gets called by Autofac runtime.
        /// Registers services in Autofac IoC directly.
        /// </summary>
        protected internal virtual void RegisterServicesInIoC(ContainerBuilder builder)
        {
            if (IsUseDatabase)
            {
                DatabaseConfigureUtility.RegisterRepositories(builder, ExecutingAssembly);
                DatabaseConfigureUtility.RegisterUnitOfWork(builder, EfCoreContextType);

                // Domain events are dispatched when entities are saving to the database from the UnitOfWork class.
                // Therefore, at the moment, we register only in those services where there is a database.
                RegisterDomainEvent(builder);
            }

            // TODO: Yasnobulkov, JD-791, After non mock implementation will be done need correct type in IoC
            builder.RegisterType<CheckAccessProvider>().As<ICheckAccessProvider>().SingleInstance();
            RegisterTypesThatImplementInterface(builder, typeof(ICurrentUserProvider), ExecutingAssembly);
            RegisterTypesThatImplementInterface(builder, typeof(IAppointmentConnectionInfoProvider), ExecutingAssembly);
            RegisterTypesThatImplementGenericInterface(builder, typeof(IQueryHandler<,>), ExecutingAssembly);
            RegisterTypesThatImplementGenericInterface(builder, typeof(ICommandHandler<>), ExecutingAssembly);
            RegisterTypesThatImplementGenericInterface(builder, typeof(ICommandHandler<,>), ExecutingAssembly);

            builder.Register<ICurrentUserIdProvider>(c =>
            {
                Guid? userId = null;
                c.TryResolve(out ICurrentUserProvider? currentUserProvider);
                try
                {
                    userId = currentUserProvider?.GetId();
                }
                catch
                {
                    // id already null so we do nothing
                }
                return new CurrentUserIdProvider(userId);
            });

            builder.RegisterType<SaveChangesActionFilterAttribute>().InstancePerLifetimeScope();
            builder.RegisterType<UserInfoProvider>().As<IUserInfoProvider>().InstancePerLifetimeScope();

            builder.RegisterMediatR(ExecutingAssembly);

            if (IsUseEventBus)
            { builder.AddSqsSender(); }
        }

        /// <summary>
        /// Registers services in ASP.NET IoC directly
        /// </summary>
        protected virtual void ConfigureServicesInternal(IServiceCollection services)
        {
            Log.Information(StartExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureServicesInternal));

            services.AddHttpContextAccessor();
            services.AddAutofac();

            //TODO: set the CORS policies for each service. (the policy is properly set on API Gateway level)
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            services.AddControllers()
                .AddApplicationPart(typeof(ServiceBaseController).Assembly)
                .AddControllersAsServices()
                .ConfigureJsonSerializer();

            JwtBearerSettings jwtBearerSettings = Configuration.GetSettingsAndValidate<JwtBearerSettings, JwtBearerSettingsValidator>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => ConfigureJwtBearer(options, jwtBearerSettings));
            services.AddAuthorization();

            ConfigureAuthorization(services);
            ConfigureAutoMapper(services);
            ConfigureFluentValidation(services);
            ConfigureDatabase(services, DatabaseConnectionString);
            ConfigureExceptionHandling(services);
            ConfigureResponseBodyLogIgnore(services);
            if (IsUseEventBus)
            {
                ConfigureEventBus(services);
            }

            var serviceStartupSettings = Configuration.GetSettings<ServiceStartupSettings>() ?? ServiceStartupSettings.Default;
            var restServiceSettings = RefitConfigureUtility.GetRestServiceSettingsOrDefault(Configuration);
            var headerPropagationSettings = Configuration.GetSettings<HeaderPropagationSettings>() ?? HeaderPropagationSettings.Default;

            services.ConfigureSwagger(ExecutingAssemblyName, serviceStartupSettings, jwtBearerSettings.Authority);
            services.ConfigureRestServices(ExecutingAssembly, restServiceSettings, headerPropagationSettings);
            services.ConfigureLogUserIdHeaderMiddleware(Configuration);
            services.ConfigureCorrelationId(Configuration);
            services.ConfigureRequestResponseLogging(Configuration);
            services.ConfigureInboundHeaderPropagation(headerPropagationSettings);

            ConfigureHealthCheck(services);

            Log.Information(EndExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureServicesInternal));
        }

        protected virtual IHealthChecksBuilder ConfigureHealthCheck(IServiceCollection services)
        {
            SqsSettings? sqsSettings = IsUseEventBus
                ? Configuration.GetSettingsAndValidate<SqsSettings, SqsSettingsValidator>()
                : null;

            Dictionary<string, RestServiceSettings> restServiceSettings =
                RefitConfigureUtility.GetRestServiceSettingsOrDefault(Configuration);

            return services.ConfigureHealthCheckServices(IsUseDatabase, DatabaseConnectionString,
                IsUseEventBus, Configuration.GetSettings<SqsSettings>(), restServiceSettings);
        }

        protected virtual void ConfigureJwtBearer(JwtBearerOptions options, JwtBearerSettings jwtBearerSettings)
        {
            Log.Information(StartExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureJwtBearer));

            JwtBearerMapping.Mapper.Map(jwtBearerSettings, options);

            Log.Information(EndExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureJwtBearer));
        }

        protected virtual void ConfigureAutoMapper(IServiceCollection services)
        {
            if (!IsUseAutomapper)
            { return; }

            Log.Information(StartExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureAutoMapper));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(ExecutingAssembly);
                cfg.ShouldMapProperty = p => p.GetMethod?.IsPublic == true || p.GetMethod?.IsPrivate == true;
            });

            services.AddSingleton(config.CreateMapper());

            Log.Information(EndExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureAutoMapper));
        }

        /// <summary>
        /// Register validators from ValidationAssemblies property if it is not empty
        /// </summary>
        protected virtual void ConfigureFluentValidation(IServiceCollection services)
        {
            Log.Information(StartExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureFluentValidation));

            if (ValidationAssemblies.Length <= 0)
            {
                Log.Warning("No validation assemblies was added");
                return;
            }

            services.AddFluentValidation(c =>
            {
                c.RegisterValidatorsFromAssemblies(ValidationAssemblies);
                c.RegisterValidatorsFromAssembly(typeof(ServiceStartupBase).Assembly); //register validators from ServiceStartup
            });

            Log.Information(EndExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureFluentValidation));
        }

        protected virtual void ConfigureAuthorization(IServiceCollection services)
        {
            if (!IsUseAuthorization)
            { return; }

            Log.Information(StartExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureAuthorization));

            services.AddHttpClient<IUserInfoProvider>(); //typed HTTP client for the user info provider
            services.ConfigureSettings<UserInfoProviderSettings>(Configuration);
            services.ConfigureSettings<JwtBearerClaimsSettings>(Configuration);

            Log.Information(EndExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureAuthorization));
        }

        protected virtual void ConfigureExceptionHandling(IServiceCollection services)
        {
            Log.Information(StartExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureExceptionHandling));

            services.ConfigureDictionarySettings<InfoLogExceptionFilterSettings>(Configuration, true);

            Log.Information(EndExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureExceptionHandling));
        }

        protected virtual void ConfigureResponseBodyLogIgnore(IServiceCollection services)
        {
            Log.Information(StartExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureResponseBodyLogIgnore));

            services.ConfigureDictionarySettings<IgnoreLoggingResponseBodySettings>(Configuration, true);

            Log.Information(EndExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureResponseBodyLogIgnore));
        }

        protected virtual void ConfigureEventBus(IServiceCollection services)
        {
            Log.Information(StartExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureEventBus));

            services.ConfigureSettings<SqsSettings, SqsSettingsValidator>(Configuration);

            Log.Information(EndExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureEventBus));
        }

        /// <summary>
        /// Configure Entity Framework settings and register EF Core context
        /// </summary>
        protected virtual void ConfigureDatabase(IServiceCollection services, string connectionString)
        {
            if (!IsUseDatabase)
            { return; }

            Log.Information(StartExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureDatabase));

            DatabaseConfigureUtility.ConfigureEfCoreDbContextSettings(services, Configuration);
            DatabaseConfigureUtility.ConfigureServices(services, EfCoreContextType, connectionString); //pass as nullable to avoid dependencies with implementation property IsUseDatabase (avoid breaking the encapsulation)

            Log.Information(EndExecutingMessage, nameof(ServiceStartupBase), nameof(ConfigureDatabase));
        }

        private void RegisterTypesThatImplementInterface(ContainerBuilder builder, Type interfaceType, params Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies)
                .Where(x => x.GetInterfaces().Contains(interfaceType))
                .AsImplementedInterfaces()
                .InstancePerDependency();
        }

        private void RegisterTypesThatImplementGenericInterface(ContainerBuilder builder, Type interfaceType, params Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies)
                .Where(x => x.ImplementsGenericInterface(interfaceType))
                .AsImplementedInterfaces()
                .InstancePerDependency();
        }

        private static void RegisterDomainEvent(ContainerBuilder builder)
        {
            builder.RegisterType<DomainEventPublisher>().As<IDomainEventPublisher>().InstancePerLifetimeScope();
        }
    }
}
