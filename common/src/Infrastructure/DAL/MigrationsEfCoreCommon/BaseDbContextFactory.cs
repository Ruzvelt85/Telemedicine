using JetBrains.Annotations;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Telemedicine.Common.Infrastructure.DAL.MigrationsEfCoreCommon
{
    /// <summary>
    /// It's implementation of the EF interface which generate DB Context when developer uses CLI
    /// </summary>
    [PublicAPI]
    public abstract class BaseDbContextFactory<TDbContext> : IDesignTimeDbContextFactory<TDbContext>
        where TDbContext : DbContext
    {
        public TDbContext CreateDbContext(string[]? args)
        {
            var configuration = ConfigurationUtilities.GetConfigurationBuilder().Build();
            var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
            SetupDbOptions(optionsBuilder, configuration);

            Serilog.ILogger logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
            ILoggerFactory loggerFactory = new LoggerFactory();

            return CreateDbContext(optionsBuilder.Options, configuration.GetSettings<EfCoreDbContextSettings>() ?? EfCoreDbContextSettings.Default, loggerFactory.AddSerilog(logger));
        }

        /// <summary>
        /// Gets connection string to database
        /// </summary>
        private static string GetConnectionString(IConfiguration configuration)
        {
            return configuration.GetConnectionString("Main");
        }

        /// <summary>
        /// Set up <see cref="DbContextOptions"/>
        /// </summary>
        protected virtual void SetupDbOptions(DbContextOptionsBuilder<TDbContext> optionsBuilder, IConfiguration configuration) =>
            optionsBuilder.UseNpgsql(GetConnectionString(configuration),
                    optionsAction => optionsAction.MigrationsAssembly(GetType().Assembly.FullName))
                .UseSnakeCaseNamingConvention();

        /// <summary>
        /// Create concrete <see cref="DbContext"/>
        /// </summary>
        protected abstract TDbContext CreateDbContext(DbContextOptions<TDbContext> options, EfCoreDbContextSettings settings, ILoggerFactory loggerFactory);
    }
}
