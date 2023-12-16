using System;
using System.Reflection;
using Autofac;
using AutoMapper.Internal;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Common.Infrastructure.Patterns.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities
{
    public class DatabaseConfigureUtility
    {
        /// <summary>
        /// Register in Autofac container all implementation of IReadRepository and IWriteRepository for specific Assembly
        /// </summary>
        public static void RegisterRepositories(ContainerBuilder builder, Assembly executingAssembly)
        {
            foreach (var repositoryInterface in new[] { typeof(IReadRepository<>), typeof(IWriteRepository<>) })
            {
                {
                    builder.RegisterAssemblyTypes(executingAssembly)
                        .Where(x => x.ImplementsGenericInterface(repositoryInterface))
                        .AsImplementedInterfaces()
                        .InstancePerDependency();
                }
            }
        }

        /// <summary>
        /// Register in Autofac container instance of UnitOfWork for specific EF Core context
        /// </summary>
        public static void RegisterUnitOfWork(ContainerBuilder builder, Type? efCoreContextType)
        {
            var methodInfo = typeof(DatabaseConfigureUtility).GetMethod(nameof(DatabaseConfigureUtility.AddUnitOfWork));

            if (CheckEfCoreContextType(efCoreContextType))
            {
                methodInfo = methodInfo!.MakeGenericMethod(efCoreContextType!);
                methodInfo.Invoke(null, new object?[] { builder });
            }
        }

        /// <summary>
        /// Register specific instance of EF Core context
        /// </summary>
        public static void ConfigureServices(IServiceCollection services, Type? efCoreContextType, string connectionString)
        {
            var methodInfo = typeof(DatabaseConfigureUtility).GetMethod(nameof(DatabaseConfigureUtility.AddEfContext));

            if (CheckEfCoreContextType(efCoreContextType))
            {
                methodInfo = methodInfo!.MakeGenericMethod(efCoreContextType!);
                methodInfo.Invoke(null, new object?[] { services, connectionString });
            }
        }

        /// <summary>
        /// Generic method for registration specific  instance of EF Core context; must be public
        /// </summary>
        public static void AddEfContext<T>(IServiceCollection services, string connectionString) where T : DbContext
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            services.AddDbContext<T>(options => GetOptions(options, connectionString), ServiceLifetime.Scoped); // TODO Pokhunov JD-381 set per dependency
        }

        public static DbContextOptionsBuilder GetOptions(DbContextOptionsBuilder builder, string connectionString)
        {
            return builder
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        }

        /// <summary>
        /// Generic method for registration UnitOfWork for specific instance of EF Core context; must be public
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        public static void AddUnitOfWork<T>(ContainerBuilder builder) where T : EfCoreDbContext
        {
            builder.RegisterType<UnitOfWork<T>>().As<IUnitOfWork>().InstancePerDependency();
        }

        /// <summary>
        /// Register settings for Ef Core in ASP.NET Core IoC container
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection ConfigureEfCoreDbContextSettings(IServiceCollection services, IConfiguration config)
        {
            return services.ConfigureSettings<EfCoreDbContextSettings>(config);
        }

        /// <summary>
        /// Checks that parameter efCoreContextType is not null and is  a subclass of EfCoreDbContext
        /// </summary>
        private static bool CheckEfCoreContextType(Type? efCoreContextType)
        {
            if (efCoreContextType is null || !efCoreContextType.IsSubclassOf(typeof(EfCoreDbContext)))
                throw new InvalidOperationException("IsUseDatabase is true, but ContextType is null or incorrect type");

            return true;
        }
    }
}
