using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Settings;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities.Authorization
{
    /// <inheritdoc cref="IUserInfoProvider"/>
    internal class UserInfoProvider : IUserInfoProvider
    {
        private readonly HttpClient _httpClient;
        private readonly UserInfoProviderSettings _settings;

        public UserInfoProvider(IOptionsSnapshot<UserInfoProviderSettings> settingsOptionsSnapshot, HttpClient httpClient)
        {
            _settings = settingsOptionsSnapshot.Value;
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, string>?> GetUserInfoAsync(string token)
        {
            IInternalUserInfoProvider internalUserInfoProvider = GetProviderOfType();
            string userInfoUrl = GetUserInfoUrl();
            return await internalUserInfoProvider.GetInfoAsync(userInfoUrl, token);
        }

        private string GetUserInfoUrl()
        {
            //TODO: Check if it's possible to get the openid-connect userinfo endpoint URL from here to use as a default value when URL is not set
            /*OpenIdConnectConfiguration config = await context.Options.ConfigurationManager.GetConfigurationAsync(default); //TokenValidatedContext.JwtBearerOptions
            string userInfoEndpointUrl = config.UserInfoEndpoint; */
            return _settings.Url ?? throw new ArgumentNullException(nameof(_settings.Url), "User info endpoint URL can't be null");
        }

        private IInternalUserInfoProvider GetProviderOfType()
        {
            switch (_settings.UserInfoProviderType)
            {
                case UserInfoProviderType.CognitoApi:
                    return new CognitoApiInternalUserInfoProvider(_httpClient);
                default:
                    return new OpenIdConnectInternalUserInfoProvider(_httpClient);
            }
        }
    }
}
