using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities.Authorization
{
    /// <summary>
    /// Provides additional user info from openid-connect userinfo endpoint.
    /// </summary>
    internal class OpenIdConnectInternalUserInfoProvider : IInternalUserInfoProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger = Log.ForContext<OpenIdConnectInternalUserInfoProvider>();

        public OpenIdConnectInternalUserInfoProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Get the user info from openid-connect userinfo endpoint. The user info is taken for a user whose <paramref name="token"/> is used.
        /// </summary>
        /// <param name="userInfoEndpointUri">uri of openid-connect userinfo endpoint</param>
        /// <param name="token">token of the user</param>
        /// <returns> null if couldn't get the user info from <paramref name="userInfoEndpointUri"/>, otherwise returns Dictionary where Key is claim type, and value is claim value</returns>
        public async Task<Dictionary<string, string>?> GetInfoAsync(string userInfoEndpointUri, string token)
        {
            try
            {
                using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, userInfoEndpointUri);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.Error("Couldn't get a successful response from {UserInfoEndpointUri}. Status code: {StatusCode}, content: {Content}", userInfoEndpointUri, response.StatusCode, response.Content);
                    return null;
                }

                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An unexpected error occurred while trying to get user info from the UserInfo endpoint");
            }

            return null;
        }
    }
}
