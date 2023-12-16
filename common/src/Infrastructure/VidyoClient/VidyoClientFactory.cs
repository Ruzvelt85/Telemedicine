using System;
using Serilog;
using VidyoService;

namespace Telemedicine.Common.Infrastructure.VidyoClient
{
    ///<inheritdoc cref="IVidyoClientFactory"/>
    internal class VidyoClientFactory : IVidyoClientFactory
    {
        private readonly ILogger _logger = Log.ForContext<VidyoClientFactory>();

        ///<inheritdoc cref="IVidyoClientFactory.Create"/>
        public VidyoPortalUserServicePortTypeClient Create(VidyoServiceConnectionSettings settings)
        {
            _logger.Debug("Creating Vidyo client...");
            return new VidyoPortalUserServicePortTypeClient(settings.Url, settings.UserName, settings.UserPassword, TimeSpan.FromMilliseconds(settings.Timeout));
        }
    }
}
