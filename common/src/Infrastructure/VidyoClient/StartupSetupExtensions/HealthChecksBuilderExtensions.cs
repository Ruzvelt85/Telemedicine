using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Telemedicine.Common.Infrastructure.VidyoClient.StartupSetupExtensions
{
    public static class HealthChecksBuilderExtensions
    {
        private const string DefaultName = "VidyoHealthCheck";

        /// <summary> 
        /// Checks the connection to Vidyo service
        /// </summary>
        /// <param name="healthCheckBuilder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'VidyoHealthCheck' will be used for the name.</param>
        /// <param name="failureStatus">The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then  the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.</param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional System.TimeSpan representing the timeout of the check.</param>
        /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
        public static IHealthChecksBuilder AddVidyoService(this IHealthChecksBuilder healthCheckBuilder,
            string? name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string>? tags = default,
            TimeSpan? timeout = default)
        {
            return healthCheckBuilder.AddCheck<VidyoClientHealthCheck>(
                name ?? DefaultName,
                failureStatus,
                tags,
                timeout);
        }
    }
}
