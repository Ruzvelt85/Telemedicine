using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using JetBrains.Annotations;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase;
using Telemedicine.Common.Infrastructure.Patterns.EventBus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup
{
    [PublicAPI]
    public abstract partial class ServiceStartupBase
    {
        private const string AppEnvironmentVariablePrefix = "APP_";

        private IHostApplicationLifetime ApplicationLifetime { get; set; }

        public IWebHostEnvironment HostingEnvironments { get; set; }

        public IConfiguration Configuration { get; }

        public StartupLogging StartupLogging { get; }

        protected virtual Type? EfCoreContextType => null;

        private static string AssemblyDirectory => Path.GetDirectoryName(ReflectionUtilities.ExecutingAssembly.Location) ?? string.Empty;

        /// <summary>
        /// Name of the executable assembly
        /// </summary>
        protected static string ExecutingAssemblyName => ReflectionUtilities.GetExecutingAssemblyName() ?? string.Empty;

        /// <summary>
        /// Return executing assembly
        /// </summary>
        /// <remarks>Made virtual for testing needs, do not override in the production code</remarks>
        protected virtual Assembly ExecutingAssembly => ReflectionUtilities.ExecutingAssembly;

        /// <summary>
        /// Assemblies that contains Fluent Validation rules
        /// </summary>
        protected virtual Assembly[] ValidationAssemblies => Array.Empty<Assembly>();

        /// <summary>
        /// To use Automapper
        /// </summary>
        protected virtual bool IsUseAutomapper { get; set; } = true;

        /// <summary>
        /// To use database
        /// </summary>
        protected virtual bool IsUseDatabase => EfCoreContextType != null;

        /// <summary>
        /// if set to true will register necessary services for authorization
        /// </summary>
        protected virtual bool IsUseAuthorization { get; set; } = false;

        /// <summary>
        /// Set it to true if the service needs to use message queue. It'll register the required classes like <see cref="IEventBusPublisher"/>
        /// </summary>
        protected virtual bool IsUseEventBus => false;

        /// <summary>
        /// Connection to database
        /// </summary>
        protected string DatabaseConnectionString => Configuration.GetConnectionString("Main");

        //Pragma here, because of we don't want extend parameters list of constructor.
        //Further parameter changes will require child class constructors refactoring
#pragma warning disable 8618
        protected ServiceStartupBase(IConfiguration configuration)
#pragma warning restore 8618
        {
            Configuration = configuration;
            StartupLogging = new StartupLogging(configuration);
        }

        /// <summary>
        /// Used to create a host builder.
        /// In the Main method, call Build() and Run() after this method
        /// </summary>
        /// <typeparam name="T">Startup type</typeparam>
        public static async Task BuildHost<T>(string[] args) where T : class
        {
            try
            {
                var service = Host.CreateDefaultBuilder(args)
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .UseSerilog()
                    // ReSharper disable once UnusedParameter.Local
                    .ConfigureAppConfiguration((hostingContext, builder) =>
                    {
                        //!!!WARNING!!! Order of configuration files very important. Configuration records overwrite each other.
                        builder
                            .SetBasePath(AssemblyDirectory)
                            .AddJsonFile("appsettings.common.logging.json", true, true)
                            .AddJsonFile("appsettings.common.json", true, true)
                            .AddJsonFile($"appsettings.common.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                            // <Space for general settings - add new settings before appsettings.json и appsettings.ENVIRONMENT.json>
                            .AddJsonFile("appsettings.json", true, true)
                            .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                            .AddEnvironmentVariables()
                            .AddEnvironmentVariables(AppEnvironmentVariablePrefix)
                            .AddCommandLine(args);
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<T>();
                    }).Build();

                Log.Information("Service is configured successfully!");

                await service.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");

                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
