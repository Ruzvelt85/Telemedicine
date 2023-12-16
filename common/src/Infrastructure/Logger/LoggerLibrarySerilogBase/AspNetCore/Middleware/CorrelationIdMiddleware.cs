using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Serilog;
using Serilog.Context;
using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.CorrelationId;

namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.Middleware
{
    /// <summary>
    /// Middleware to retrieve and return CorrelationId.
    /// Retrieve correlation id from HTTP Headers. If there isn't correlation then middleware generate it.
    /// </summary>
    public class CorrelationIdMiddleware
    {
        private readonly CorrelationIdSettings _settings;
        private readonly RequestDelegate _next;
        private readonly ILogger _logger = Log.ForContext<CorrelationIdMiddleware>();
        private readonly ICorrelationIdGenerator _correlationIdGenerator;

        public CorrelationIdMiddleware(
            RequestDelegate next,
            IOptions<CorrelationIdSettings> settings,
            ICorrelationIdGenerator correlationIdGenerator
        )
        {
            _next = next;
            _correlationIdGenerator = correlationIdGenerator;
            _settings = settings.Value;

            _logger.Debug("{MiddlewareName} initialized", nameof(CorrelationIdMiddleware));
        }

        [UsedImplicitly]
        public async Task InvokeAsync(HttpContext httpContext, ICorrelationIdAccessor correlationIdAccessor)
        {
            if (!IsAllowedToInvoke())
            {
                _logger.Debug("{MiddlewareName} is disabled", nameof(CorrelationIdMiddleware));
                await _next(httpContext);
                return;
            }

            _logger.Debug("{MiddlewareName} is enabled", nameof(CorrelationIdMiddleware));
            string correlationId = string.Empty;
            try
            {
                correlationId = GetCorrelationId(httpContext);

                correlationIdAccessor.CorrelationId = correlationId;
                httpContext.TraceIdentifier = correlationId;
                TryAddCorrelationIdToRequest(httpContext, correlationId);

                TryAddCorrelationIdToResponse(httpContext, correlationId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected exception occurred in CorrelationIdMiddleware");
                Debug.Fail("Unexpected exception occurred in CorrelationIdMiddleware");
            }
            finally
            {
                await AddCorrelationIdToLogScopeAndContinue(httpContext, correlationId);
            }
        }

        /// <summary>
        /// Check is possible to execute middleware
        /// If <see cref="CorrelationIdSettings.IsEnabled"/> is <para>true</para> and
        /// settings is valid:  <see cref="CorrelationIdSettings.LogPropertyName"/> and <see cref="CorrelationIdSettings.LogPropertyName"/> are not empty
        /// </summary>
        /// <returns></returns>
        private bool IsAllowedToInvoke()
        {
            if (!_settings.IsEnabled)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(_settings.HeaderName))
            {
                _logger.Warning("HeaderName in settings is empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_settings.LogPropertyName))
            {
                _logger.Warning("LogPropertyName in settings is empty");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get correlations Id from request header
        /// if <see cref="CorrelationIdSettings.AcceptIncomingHeader"/> is TRUE
        /// and <see cref="CorrelationIdSettings.HeaderName"/> is NOT EMPTY
        /// In the opposite case, a new correlation Id is generated
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns>Existing correlationId from request header or generated correlationId</returns>
        private string GetCorrelationId(HttpContext httpContext)
        {
            var correlationId = string.Empty;
            if (_settings.AcceptIncomingHeader)
            {
                var hasCorrelationIdHeader = httpContext.Request.Headers.TryGetValue(_settings.HeaderName, out var correlationIdHeader);
                if (hasCorrelationIdHeader)
                {
                    correlationId = correlationIdHeader.FirstOrDefault();
                }
            }

            if (string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = _correlationIdGenerator.Generate();
                _logger.Debug("CorrelationId was generated with value ({CorrelationId})", correlationId);
            }

            return correlationId;
        }

        /// <summary>
        /// Try to add correlation id to request headers
        /// if <see cref="CorrelationIdSettings.HeaderName"/> is NOT EMPTY
        /// </summary>
        private void TryAddCorrelationIdToRequest(HttpContext httpContext, string correlationId)
        {
            TryAddCorrelationIdToHeaders(httpContext.Request.Headers, correlationId, true);
        }

        /// <summary>
        /// Try to add correlation id to headers
        /// if <see cref="CorrelationIdSettings.HeaderName"/> is NOT EMPTY
        /// </summary>
        private void TryAddCorrelationIdToHeaders(IHeaderDictionary headers, string correlationId, bool isRequest)
        {
            if (!headers.ContainsKey(_settings.HeaderName))
            {
                _logger.Debug("Adding CorrelationId ({CorrelationId}) to {RequestOrResponse} headers with name {HeaderName}", correlationId, (isRequest ? "request" : "response"), _settings.HeaderName);
                headers.Add(_settings.HeaderName, new StringValues(correlationId));
            }
        }

        /// <summary>
        /// Try to add correlation id to response
        /// if <see cref="CorrelationIdSettings.IsIncludeInResponse"/> is TRUE
        /// and <see cref="CorrelationIdSettings.HeaderName"/> is NOT EMPTY
        /// </summary>
        private void TryAddCorrelationIdToResponse(HttpContext httpContext, string correlationId)
        {
            if (!_settings.IsIncludeInResponse)
            {
                _logger.Debug("Property {PropertyName} in settings set to false", _settings.IsIncludeInResponse);
                return;
            }

            httpContext.Response.OnStarting(() =>
            {
                TryAddCorrelationIdToHeaders(httpContext.Response.Headers, correlationId, false);

                return Task.CompletedTask;
            });
        }

        /// <summary>
        /// Try include correlation id to logs scope
        /// with key <see cref="CorrelationIdSettings.LogPropertyName"/>
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        private async Task AddCorrelationIdToLogScopeAndContinue(HttpContext httpContext, string correlationId)
        {
            _logger.Debug("Gonna add correlationId to serilog scope: {CorrelationId}", correlationId);
            using (LogContext.PushProperty(_settings.LogPropertyName, correlationId))
            {
                await _next(httpContext);
            }
        }
    }
}
