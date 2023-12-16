using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;

namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.Enrichers
{
    /// <summary>
    ///    Enricher for logging HTTP-request headers
    ///    Write the specified headers to property "HttpRequestHeader-[property name]"
    /// </summary>
    public class HttpRequestHeadersEnricher : ILogEventEnricher
    {
        /// <summary>
        /// Prefix for Serilog property name
        /// </summary>
        public const string HttpRequestHeadersPrefixInLog = "HttpRequestHeader-";

        /// <summary>
        /// HTTP-Header names <see cref="HttpContext.Items"/> for logging
        /// </summary>
        private readonly string[] _headersName;

        /// <summary>
        /// <see cref="HttpContext"/> accessor <see cref="IHttpContextAccessor"/> and <see cref="HttpContextAccessor"/>
        /// </summary>
        private readonly IHttpContextAccessor? _contextAccessor;

        /// <summary>
        /// <see cref="HttpContext.Items"/>
        /// </summary>
        private IHeaderDictionary? HttpRequestHeaders => _contextAccessor?.HttpContext?.Request.Headers;

        /// <summary>
        ///    Are there HTTP-header in HTTP-request
        /// </summary>
        public bool IsHeadersExists => _contextAccessor?.HttpContext?.Request.Headers.Any() ?? false;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="contextAccessor">HttpContext accessor <see cref="IHttpContextAccessor"/> and <see cref="HttpContextAccessor"/></param>
        /// <param name="httpHeadersName">HTTP request header name from <see cref="HttpContext.Items"/> for logging</param>
        public HttpRequestHeadersEnricher(IHttpContextAccessor contextAccessor, IEnumerable<string?>? httpHeadersName)
        {
            _contextAccessor = contextAccessor;
            _headersName = httpHeadersName?.Select(item => item?.Trim() ?? string.Empty)
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .ToArray() ?? Array.Empty<string>();
        }

        /// <summary>
        /// Add value of http headers to log
        /// </summary>
        /// <param name="logEvent">log event for enriching</param>
        /// <param name="propertyFactory">Class factory to create properties for log event</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (!IsHeadersExists)
            {
                return;
            }
            foreach (var headerName in _headersName)
            {
                CreateProperty(headerName, logEvent, propertyFactory);
            }
        }

        /// <summary>
        /// Add property with HTTP header value to log
        /// </summary>
        /// <param name="headerName">HTTP header name</param>
        /// <param name="logEvent">Log event for enriching</param>
        /// <param name="propertyFactory">Class factory to create properties for log event</param>
        private void CreateProperty(string headerName, LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            try
            {
                if (HttpRequestHeaders != null && HttpRequestHeaders.TryGetValue(headerName, out var value))
                {
                    var property = propertyFactory.CreateProperty($"{HttpRequestHeadersPrefixInLog}{headerName}", value);
                    logEvent.AddPropertyIfAbsent(property);
                }
            }
            catch (Exception exception)
            {
                SelfLog.WriteLine($"Error in {GetType().FullName}. Exception={exception}");
            }
        }
    }
}
