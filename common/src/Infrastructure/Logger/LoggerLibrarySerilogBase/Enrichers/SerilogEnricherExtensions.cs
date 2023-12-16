using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Configuration;

namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.Enrichers
{
    /// <summary>
    ///     Configuration for serilog extensions
    /// </summary>
    public static class SerilogEnricherExtensions
    {
        /// <summary>
        /// Enrich log by HTTP-headers
        /// </summary>
        /// <param name="enrichmentConfiguration">Logger enrichment configuration.</param>
        /// <param name="httpHeadersName">List of http-header names, which we are going to add </param>
        /// <returns></returns>
        public static LoggerConfiguration WithHttpRequestHeaders(
            this LoggerEnrichmentConfiguration enrichmentConfiguration,
            IEnumerable<string> httpHeadersName)
        {
            if (enrichmentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            }
            return enrichmentConfiguration.With(new HttpRequestHeadersEnricher(new HttpContextAccessor(), httpHeadersName));
        }
    }
}
