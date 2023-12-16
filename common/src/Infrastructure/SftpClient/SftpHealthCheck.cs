using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Serilog;

namespace Telemedicine.Common.Infrastructure.SftpClient
{
    public class SftpHealthCheck : IHealthCheck, IDisposable
    {
        private readonly object _sftpClientLock = new();
        private ISftpClient? _sftpClient;
        private readonly IOptionsMonitor<SftpClientSettings> _sftpClientOptionsMonitor;
        private readonly Func<IOptionsMonitor<SftpClientSettings>, ISftpClient> _sftpClientFactory;
        private readonly ILogger _logger = Log.ForContext<SftpHealthCheck>();

        [PublicAPI]
        public SftpHealthCheck(IOptionsMonitor<SftpClientSettings> sftpClientOptionsMonitor) : this(sftpClientOptionsMonitor, DefaultSftpClientFactory)
        {
        }

        // Constructor for unit tests
        internal SftpHealthCheck(IOptionsMonitor<SftpClientSettings> sftpClientOptionsMonitor, Func<IOptionsMonitor<SftpClientSettings>, ISftpClient> sftpClientFactory)
        {
            _logger.Debug("Initializing SFTP client.");
            _sftpClientOptionsMonitor = sftpClientOptionsMonitor ?? throw new ArgumentNullException(nameof(sftpClientOptionsMonitor));
            _sftpClientFactory = sftpClientFactory ?? throw new ArgumentNullException(nameof(sftpClientFactory));
            _sftpClientOptionsMonitor.OnChange(ChangeSettings);
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            _logger.Debug("'{HealthCheck}' has been called.", nameof(SftpHealthCheck));
            try
            {
                bool isSuccess;
                lock (_sftpClientLock)
                {
                    _sftpClient ??= _sftpClientFactory(_sftpClientOptionsMonitor)
                                    ?? throw new InvalidOperationException("Cannot create SFTP client.");

                    isSuccess = _sftpClient.TryConnect();
                }

                var result = isSuccess
                    ? HealthCheckResult.Healthy()
                    : new HealthCheckResult(context.Registration.FailureStatus);
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var result = new HealthCheckResult(context.Registration.FailureStatus, ex.Message, ex);
                return Task.FromResult(result);
            }
        }

        public void Dispose()
        {
            lock (_sftpClientLock)
            {
                _sftpClient?.Dispose();
            }
        }

        private void ChangeSettings(SftpClientSettings sftpClientSettings)
        {
            _logger.Debug("Change settings '{SettingsName}'", nameof(SftpClientSettings));
            lock (_sftpClientLock)
            {
                _sftpClient?.Dispose();
                _sftpClient = _sftpClientFactory(_sftpClientOptionsMonitor);
            }
        }

        private static ISftpClient DefaultSftpClientFactory(IOptionsMonitor<SftpClientSettings> sftpClientOptionsMonitor)
        {
            var factory = new ExternalSftpClientFactory();
            var sftpClient = new SftpClientImpl(factory, sftpClientOptionsMonitor);
            return sftpClient;
        }
    }
}
