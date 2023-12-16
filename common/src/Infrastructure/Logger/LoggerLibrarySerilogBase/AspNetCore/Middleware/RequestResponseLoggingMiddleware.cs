using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.Settings;
using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serilog;

namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.Middleware
{
    /// <summary>
    /// Middleware to log request and response
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly ILogger _logger = Log.ForContext<RequestResponseLoggingMiddleware>();
        private readonly RequestDelegate _next;
        private readonly IOptionsMonitor<RequestResponseLoggingSettings> _loggingSettings;
        private readonly IOptionsMonitor<Dictionary<string, IgnoreLoggingResponseBodySettings>> _ignoreLoggingResponseBodySettings;

        public RequestResponseLoggingMiddleware(RequestDelegate next, IOptionsMonitor<RequestResponseLoggingSettings> loggingSettings,
            IOptionsMonitor<Dictionary<string, IgnoreLoggingResponseBodySettings>> ignoreLoggingResponseBodySettings)
        {
            _next = next;
            _ignoreLoggingResponseBodySettings = ignoreLoggingResponseBodySettings;
            _loggingSettings = loggingSettings;

            _logger.Debug("{MiddlewareName} initialized", nameof(RequestResponseLoggingMiddleware));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Caching settings because we are using IOptionsMonitor and state
            // of settings can change through log processing
            var settingsCache = _loggingSettings.CurrentValue;

            if (!settingsCache.IsEnabled)
            {
                _logger.Debug("{MiddlewareName} is disabled", nameof(RequestResponseLoggingMiddleware));
                await _next(context);
                return;
            }

            _logger.Debug("{MiddlewareName} is enabled", nameof(RequestResponseLoggingMiddleware));

            await TryLogRequest(context, settingsCache);

            if (!settingsCache.IsLogResponse)
            {
                await _next(context);
                return;
            }

            if (!IsNeedToLogResponseBody(context))
            {
                await _next(context);

                LogResponseWithoutBody(context);
                return;
            }

            await TryLogResponseWithBody(context, _next);
        }

        private async Task TryLogResponseWithBody(HttpContext context, RequestDelegate next)
        {
            var originalBodyStream = context.Response.Body;

            //This is the only one right way to log response body
            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                context.Response.Body = originalBodyStream;
                ExceptionDispatchInfo.Capture(exception).Throw();
            }

            try
            {
                await LogResponseWithBody(context);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Unexpected exception occurred during log response: {Exception}", e);
                Debug.Fail($"Unexpected exception occurred during log response: {e}");
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        /// <summary>
        /// Log request details
        /// </summary>
        private async Task TryLogRequest(HttpContext context, RequestResponseLoggingSettings settingsCache)
        {
            try
            {
                var requestHeaders = string.Empty;
                var requestBody = string.Empty;

                if (settingsCache.IsLogRequestHeader)
                {
                    requestHeaders = FormatHeaders(context.Request.Headers);
                }

                if (settingsCache.IsLogRequestBody)
                {
                    requestBody = await FormatRequestBody(context.Request);
                }

                _logger.Information("Request URL: {Scheme} {Host}{Path} {QueryString} HttpRequestHeaders:{HttpRequestHeaders} HttpRequestBody:{HttpRequestBody}",
                    context.Request.Scheme, context.Request.Host, context.Request.Path, context.Request.QueryString, requestHeaders, requestBody);
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Failed to log request body. Exception: {Exception}", exception);
                Debug.Fail($"Failed to log request body. Exception: {exception}");
            }
        }

        private async Task LogResponseWithBody(HttpContext context)
        {
            var responseHeaders = FormatHeaders(context.Response.Headers);

            var response = await FormatResponseBody(context.Response);

            _logger.Information(
                "Response from {Scheme} {Host}{Path} {QueryString} HttpResponseHeaders:{HttpResponseHeaders} HttpResponseBody:{HttpResponseBody}",
                context.Request.Scheme, context.Request.Host, context.Request.Path,
                context.Request.QueryString, responseHeaders, response);
        }

        private void LogResponseWithoutBody(HttpContext context)
        {
            var responseHeaders = FormatHeaders(context.Response.Headers);

            _logger.Information(
                "Response from {Scheme} {Host}{Path} {QueryString} HttpResponseHeaders:{HttpResponseHeaders} HttpResponseBody: Ignored",
                context.Request.Scheme, context.Request.Host, context.Request.Path,
                context.Request.QueryString, responseHeaders);
        }

        private bool IsNeedToLogResponseBody(HttpContext context)
        {
            var path = (context.GetEndpoint() as Microsoft.AspNetCore.Routing.RouteEndpoint)?.RoutePattern.RawText;
            if (path == null)
            {
                return true;
            }

            return !LoggerSettingsUtilities.HasIgnoreLoggingBodySettings(_ignoreLoggingResponseBodySettings.CurrentValue, context.Request.Method, path);
        }

        /// <summary>
        /// Get all headers as string
        /// </summary>
        private static string FormatHeaders(IHeaderDictionary httpHeaders)
        {
            var headers = new StringBuilder(1024); // it is better to use 1024, because by default capacity is 16
            foreach (var (key, value) in httpHeaders)
            {
                headers.Append($"[{key}:{value}]");
            }
            return headers.ToString();
        }

        /// <summary>
        /// Get Body as string from HttpRequest
        /// </summary>
        private static async Task<string> FormatRequestBody(HttpRequest request)
        {
            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength, CultureInfo.InvariantCulture)];

            await using (var ms = new MemoryStream())
            {
                int read;
                while ((read = await request.Body.ReadAsync(buffer, CancellationToken.None)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
            }

            var bodyAsText = Encoding.UTF8.GetString(buffer);

            // assign the read body back to the request body, which is allowed because of EnableBuffering()
            request.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        /// <summary>
        /// Get body from HttpResponse
        /// </summary>
        private static async Task<string> FormatResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return $"{response.StatusCode}: {text}";
        }
    }
}
