using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Serilog;
using VidyoService;

namespace Telemedicine.Common.Infrastructure.VidyoClient
{
    public class VidyoClientHealthCheck : IHealthCheck
    {
        private readonly IOptionsMonitor<VidyoServiceConnectionSettings> _vidyoClientSettings;
        private readonly ILogger _logger = Log.ForContext<VidyoClientHealthCheck>();

        [PublicAPI]
        public VidyoClientHealthCheck(IOptionsMonitor<VidyoServiceConnectionSettings> settings)
        {
            _logger.Debug("Initializing Vidyo client");
            _vidyoClientSettings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            _logger.Debug("'{HealthCheck}' has been called.", nameof(VidyoClientHealthCheck));

            VidyoPortalUserServicePortTypeClient vidyoClient = GetVidyoClient(_vidyoClientSettings);

            if (vidyoClient is null)
            { throw new InvalidOperationException($"Cannot create Vidyo client"); }

            try
            {
                // External service Vidyo does not have special method for health check. So, we use the simplest info-method (it has no parameters) to check the liveness of this service
                await vidyoClient.getUserNameAsync(new GetUserNameRequest());
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"An unexpected exception occurred while connecting to service with Url='{_vidyoClientSettings.CurrentValue.Url}', check settings", ex);
            }
            finally
            {
                vidyoClient.Close();
            }
        }

        private static VidyoPortalUserServicePortTypeClient GetVidyoClient(IOptionsMonitor<VidyoServiceConnectionSettings> settings)
        {
            return new VidyoClientFactory().Create(settings.CurrentValue);
        }
    }
}
