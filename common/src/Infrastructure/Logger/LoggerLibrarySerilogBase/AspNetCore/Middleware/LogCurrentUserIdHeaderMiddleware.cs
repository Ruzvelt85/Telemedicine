using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Context;

namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.Middleware
{
    public class LogCurrentUserIdHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger = Log.ForContext<LogCurrentUserIdHeaderMiddleware>();

        public LogCurrentUserIdHeaderMiddleware(RequestDelegate next)
        {
            _next = next;

            _logger.Information("{MiddlewareName} initialized", nameof(LogCurrentUserIdHeaderMiddleware));
        }

        [UsedImplicitly]
        public async Task InvokeAsync(HttpContext httpContext, IOptionsSnapshot<LogCurrentUserIdHeaderSettings> settingsSnapshot, ICurrentUserIdProvider currentUserIdProvider)
        {
            LogCurrentUserIdHeaderSettings settings = settingsSnapshot.Value;
            _logger.Debug("{MiddlewareName} is enabled: {IsEnabled}", nameof(LogCurrentUserIdHeaderMiddleware), settings.IsEnabled);
            if (!settings.IsEnabled)
            {
                await _next(httpContext);
                return;
            }

            await InvokeAsyncInternal(httpContext, settings, currentUserIdProvider);
        }

        private async Task InvokeAsyncInternal(HttpContext httpContext, LogCurrentUserIdHeaderSettings settings, ICurrentUserIdProvider currentUserIdProvider)
        {
            Guid? currentUserId = null;
            try
            {
                currentUserId = currentUserIdProvider.GetId();
                if (currentUserId is null)
                { return; }

                AddCurrentUserIdToRequestHeaders(httpContext, settings, currentUserId.Value);
            }
            catch (Exception exception)
            {
                _logger.Warning(exception, "Error occurred in {Middleware}", nameof(LogCurrentUserIdHeaderMiddleware));
            }
            finally
            {
                await AddCurrentUserIdToLogScopeAndContinue(currentUserId, settings, httpContext);
            }
        }

        private void AddCurrentUserIdToRequestHeaders(HttpContext httpContext, LogCurrentUserIdHeaderSettings settings, Guid currentUserId)
        {
            _logger.Debug("Gonna add user id into request header: {UserId}", currentUserId);
            httpContext.Request.Headers.Add(settings.HeaderName, currentUserId.ToString());
        }

        private async Task AddCurrentUserIdToLogScopeAndContinue(Guid? currentUserId, LogCurrentUserIdHeaderSettings settings, HttpContext httpContext)
        {
            _logger.Debug("Gonna add user id into log scope: {UserId}", currentUserId);
            using (LogContext.PushProperty(settings.LogPropertyName, currentUserId))
            {
                await _next(httpContext);
            }
        }
    }
}
