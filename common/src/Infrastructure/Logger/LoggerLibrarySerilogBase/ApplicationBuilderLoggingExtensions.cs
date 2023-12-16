using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.CorrelationId;
using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase
{
    public static class ApplicationBuilderLoggingExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }

        public static IApplicationBuilder UseLogUserIdHeaderMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LogCurrentUserIdHeaderMiddleware>();
        }

        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CorrelationIdMiddleware>();
        }

        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app, CorrelationIdSettings settings)
        {
            return app.UseMiddleware<CorrelationIdMiddleware>(Options.Create(settings));
        }

        public static IServiceCollection ConfigureLogUserIdHeaderMiddleware(this IServiceCollection services, IConfiguration config)
        {
            return services.ConfigureSettings<LogCurrentUserIdHeaderSettings>(config);
        }

        public static IServiceCollection ConfigureCorrelationId(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();
            services.AddSingleton<ICorrelationIdGenerator, CorrelationIdGenerator>();
            return services.ConfigureSettings<CorrelationIdSettings>(config);
        }

        public static IServiceCollection ConfigureRequestResponseLogging(this IServiceCollection services, IConfiguration config)
        {
            return services.ConfigureSettings<RequestResponseLoggingSettings>(config);
        }
    }
}
