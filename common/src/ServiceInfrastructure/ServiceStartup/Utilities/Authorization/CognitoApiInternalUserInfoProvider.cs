using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities.Authorization
{
    /// <summary>
    /// Provides additional user info from Cognito API GetUser endpoint.
    /// </summary>
    internal class CognitoApiInternalUserInfoProvider : IInternalUserInfoProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger = Log.ForContext<CognitoApiInternalUserInfoProvider>();

        public CognitoApiInternalUserInfoProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Get the user info from Cognito API GetUser endpoint. The user info is taken for a user whose <paramref name="token"/> is provided.
        /// </summary>
        /// <param name="userInfoEndpointUri">uri of Cognito API GetUser endpoint</param>
        /// <param name="token">token of the user</param>
        /// <returns> null if couldn't get the user info from <paramref name="userInfoEndpointUri"/>, otherwise returns Dictionary where Key is claim type, and value is claim value</returns>
        public async Task<Dictionary<string, string>?> GetInfoAsync(string userInfoEndpointUri, string token)
        {
            try
            {
                using HttpRequestMessage request = BuildHttpRequestMessage(userInfoEndpointUri, token);
                using HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                { throw new Exception($"Couldn't get a successful response from {userInfoEndpointUri}. Status code: {response.StatusCode}, content: {response.Content}"); } //Custom exception?

                string content = await response.Content.ReadAsStringAsync();
                return GetUserClaimsFromContent(content);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An unexpected error occurred while trying to get user info from Cognito API GetUser endpoint");
            }

            return null;
        }

        private HttpRequestMessage BuildHttpRequestMessage(string userInfoEndpointUri, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, userInfoEndpointUri);
            request.Headers.Add("X-Amz-Target", "AWSCognitoIdentityProviderService.GetUser");
            request.Content = new StringContent($"{{\"AccessToken\": \"{token}\" }}");
            request.Content!.Headers.ContentType = new MediaTypeHeaderValue("application/x-amz-json-1.1");

            return request;
        }

        private Dictionary<string, string> GetUserClaimsFromContent(string content)
        {
            _logger.Verbose("We are going to retrieve user claims from the content");

            JObject jsonObj = JObject.Parse(content);
            JToken? obj = jsonObj["UserAttributes"];
            if (obj is null)
            {
                Debug.Assert(false, $"The response from Cognito API GetUser endpoint doesn't contain UserAttributes key. Content: {content}");
                throw new Exception($"The response from Cognito API GetUser endpoint doesn't contain UserAttributes key. Content: {content}");
            }

            var claimsDict = new Dictionary<string, string>();

            foreach (JToken claim in obj)
            {
                JProperty? keyProperty = claim.First as JProperty;
                JProperty? valueProperty = claim.Last as JProperty;
                JValue? keyPropertyValue = keyProperty?.Value as JValue;
                JValue? valuePropertyValue = valueProperty?.Value as JValue;

                if (keyPropertyValue is null || valuePropertyValue is null)
                {
                    Debug.Assert(false, "Couldn't get the value from keyProperty and/or valueProperty");
                    _logger.Error("Couldn't get the value from keyProperty and/or valueProperty,\nkeyProperty: {@KeyProperty}\nvalueProperty: {@ValueProperty}", keyProperty, valueProperty);
                    continue;
                }

                string? key = keyPropertyValue.Value<string>();
                string? value = valuePropertyValue.Value<string>();
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                {
                    Debug.Assert(false, "The value is null or empty string or we couldn't convert the value to string");
                    _logger.Error("The value or the key are empty key or we couldn't convert the value to string: {@Key}, value: {@Value}", key, value);
                    continue;
                }

                claimsDict.TryAdd(key, value);
            }

            _logger.Debug("We retrieved claims from the HTTP response content");
            return claimsDict;
        }
    }
}
